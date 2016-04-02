using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using EfawateerGateway.Proxy.Service;
using Gateways.Utils;
using EfawateerGateway;

// ReSharper disable CheckNamespace

namespace Gateways
// ReSharper restore CheckNamespace
{
    public class EfawateerGateway : BaseGateway, IGateway, IGateGetData
    {

        private enum PaymentType
        {
            Prepaid,
            Postpaid
        }

        public class PaymentResult
        {
            public string JoebppsTrx { get; set; }
            public int Error { get; set; }
            public DateTime StmtDate { get; set; }
            public StringList Params { get; set; }
        }

        // фатальные ошибки
        private readonly List<int> _fatalErrors = new List<int>();

        private GatewayConfig _config;

        private Guid _tokenGuid;

        private const string Bilinqrq = "BILINQRQ";
        private const string Bilpmtrq = "BILPMTRQ";
        private const string Prepadvalrq = "PREPADVALRQ";
        private const string Prepadpmtrq = "PREPADPMTRQ";
        private const string Pmtinqrq = "PMTINQRQ";
        

        public EfawateerGateway()
        {
            _config = new GatewayConfig();
        }

        public EfawateerGateway(EfawateerGateway gateway)
        {
            if(gateway._config != null)
                _config = (GatewayConfig)gateway._config.Clone();
            
            // base copy
            Copy(gateway);
        }

        public void Initialize(string data)
        {
            Audit("Initialize, GateProfileID=" + GateProfileID);

            try
            {
                _config.Load(data);
            }
            catch (Exception ex)
            {
                Audit("Initialize exception: " + ex);
            }
        }

        public void ProcessPayment(object paymentData, object operatorData, object exData)
        {
            var initial_session = string.Empty;
            Audit("Efawateer processing...");

            try
            {
                var paymentRow = paymentData as DataRow;
                if (paymentRow == null) throw new Exception("unable to extract paymentData");

                var operatorRow = operatorData as DataRow;
                if (operatorRow == null) throw new Exception("unable to extract operatorRow");

                AuditTableRows(paymentRow, operatorRow);

                initial_session = (paymentRow["InitialSessionNumber"] as string);

                var session = Guid.NewGuid().ToString();
                //var session = (paymentRow["SessionNumberEx"] is DBNull)
                //    ? ""
                //    : Convert.ToString(paymentRow["SessionNumberEx"] as string);

                var ap = (int) paymentRow["TerminalID"];
                var status = (int) paymentRow["StatusID"];
                var error = (int) paymentRow["ErrorCode"];
                var paymentParams = paymentRow["Params"] as string;
                var processDateStr = paymentRow["InitializeDateTime"].ToString();
                
                string operatorFormatString = operatorRow["OsmpFormatString"] is DBNull ? "" : operatorRow["OsmpFormatString"] as string;

                StringList parametersList;
                if (!paymentParams.Contains(";"))
                {
                    var formatedPaymentParams = paymentParams.FormatParameters(operatorFormatString);
                    parametersList = new StringList(formatedPaymentParams, ";");
                }
                else
                {
                    parametersList = new StringList(paymentParams, ";");
                    session = parametersList.Get("Session");
                }

                if (!parametersList.ContainsKey("InitializeDateTime"))
                    parametersList.Add("InitializeDateTime", processDateStr);

                var cyberplatOperatorId = (int) paymentRow["CyberplatOperatorID"];

                // отмена платежа вручную
                if (status == 103 || status == 104)
                {
                    Audit("Отмена платежа вручную");
                    PreprocessPaymentStatus(ap, initial_session, EfawateerCodeToCyberCode(error), 100, exData);
                    return;
                }
                
                if (status == 6)
                {
                    Audit("Проведение платежа");
                    var result = PaymentInquiryRequest(cyberplatOperatorId, parametersList, session);
                    PreprocessPaymentStatus(ap, initial_session, EfawateerCodeToCyberCode(result.Error), result.Error == 0 ? 7 : 100, exData);                    
                    return;
                }

                var paymentResult = new PaymentResult();
                // еще раз получить параметры платежа (первый раз во время онлайн проверке)
                var paymentType = (PaymentType) Enum.Parse(typeof (PaymentType), parametersList.Get("PaymentType"));
                try
                {
                    switch (paymentType)
                    {
                        case PaymentType.Prepaid:
                            paymentResult = PrepaidValidationRequest(cyberplatOperatorId, parametersList);
                            if (paymentResult.Error == 0)
                                paymentResult = PrepaidPaymentRequest(cyberplatOperatorId, paymentResult.Params, session);
                            break;
                        case PaymentType.Postpaid:
                            paymentResult = BillInquiryRequest(cyberplatOperatorId, parametersList);
                            if (paymentResult.Error == 0)
                                paymentResult = BillPaymentRequest(cyberplatOperatorId, parametersList, session);
                            break;
                        default:
                            throw new Exception("Неизвестный тип платежа");
                    }
                }
                catch (TimeoutException exception)
                {
                    paymentResult.Error = 100;
                    Audit("Ошибка проведения платежа (timeout) " + exception.Message);
                }
                catch (Exception exception)
                {
                    Audit("Ошибка проведения платежа " + exception.Message);
                }

                if (paymentResult.Error == 370)
                    PreprocessPaymentStatus(ap, initial_session, EfawateerCodeToCyberCode(paymentResult.Error), 100, exData);
                else
                {   
                    var s = "";
                    if (!string.IsNullOrEmpty(paymentResult.JoebppsTrx))
                        s = paymentResult.JoebppsTrx;
                    if (paymentResult.StmtDate == default (DateTime))
                        paymentResult.StmtDate = DateTime.Now;

                    PreprocessPayment(ap, initial_session, s, paymentResult.StmtDate, exData);       

                    // 1-6 - эти статусы можешь юзать, пока проведением платежа занимаешься
                    PreprocessPaymentStatus(ap, initial_session, EfawateerCodeToCyberCode(paymentResult.Error),
                        paymentResult.Error == 0 ? 6 : 101, exData);

                    if(!parametersList.ContainsKey("Session"))
                        parametersList.Add("Session", session);
                    UpdatePaymentParams(ap, initial_session, parametersList.GetParamsString(), exData);
                }

                //// ?
                //if (paymentResult.StmtDate == default (DateTime))
                //    paymentResult.StmtDate = DateTime.Now;         
            }
            catch (Exception ex)
            {
                Audit(string.Format("ProcessPayment (initial_session={0}) exception: {1}", initial_session, ex.ToString()));
            }
        }

        /// <summary>использвалось для отладки</summary>
        private void AuditTableRows(DataRow paymentRow, DataRow operatorRow)
        {
            Audit("paymentRow");
            foreach (var column in paymentRow.Table.Columns.OfType<DataColumn>())
            {
                Audit(string.Format("column {0} value '{1}'", column.ColumnName,
                    (paymentRow[column.ColumnName] is DBNull) ? "null" : paymentRow[column.ColumnName]));
            }

            Audit("operatorRow");
            foreach (var column in operatorRow.Table.Columns.OfType<DataColumn>())
            {
                try
                {
                    Audit(string.Format("column {0} value '{1}'", column.ColumnName,
                        (operatorRow[column.ColumnName] is DBNull) ? "null" : operatorRow[column.ColumnName]));
                }
                catch (Exception)
                {
                    Audit(string.Format("column {0} value 'exception'", column.ColumnName));
                }
            }
        }

        public PaymentResult PaymentInquiryRequest(int cyberplatOperatorId, StringList parametersList, string session)
        {
            var result = new PaymentResult
            {
                Error = 0
            };

            var billerCode = ExpandBillerCodeFromCyberplatOpertaroId(cyberplatOperatorId);

            var token = Authenticate();
            var request = GetRequestContent(Pmtinqrq);
            var signer = new EfawateerSigner(_config.SignCertificateThumb, _config.VerifyCertificateThumb);

            var now = DateTime.Now;
            var time = now.ToString("s");
            var guid = GenerateGuid();

            request.Element("MsgHeader").Element("TmStp").Value = time;
            request.Element("MsgHeader").Element("TrsInf").Element("SdrCode").Value = _config.CustomerCode;

            var trxInf = request.Element("MsgBody").Element("Transactions").Element("TrxInf");

            trxInf.Element("PmtGuid").Value = session;
            trxInf.Element("ParTrxID").Value = session;

            if (parametersList.ContainsKey("ValidationCode"))
                trxInf.Element("ValidationCode").Value = parametersList["ValidationCode"];
            else
                trxInf.Element("ValidationCode").Remove();

            trxInf.Element("DueAmt").Value = parametersList.Get("DueAmt");
            trxInf.Element("PaidAmt").Value = parametersList.Get("DueAmt");
            trxInf.Element("ProcessDate").Value = parametersList.Get("InitializeDateTime");
            trxInf.Element("PaymentType").Value = parametersList.Get("PaymentType");
            trxInf.Element("ServiceTypeDetails").Element("ServiceType").Value = parametersList.Get("ServiceType");

            trxInf.Element("ServiceTypeDetails").Element("PrepaidCat").Remove();

            var accInfo = trxInf.Element("AcctInfo");

            if (!parametersList.ContainsKey("BillingNo") || parametersList["BillingNo"].Contains("#"))
            {
                accInfo.Element("BillingNo").Remove();
                accInfo.Element("BillNo").Remove();
            }
            else
            {
                accInfo.Element("BillingNo").Value = parametersList["BillingNo"];
                accInfo.Element("BillNo").Value = parametersList["BillingNo"];
            }

            accInfo.Element("BillerCode").Value = billerCode.ToString(CultureInfo.InvariantCulture);

            Audit("PaymentInquiryRequest request:" + request);

            request.Element("MsgFooter").Element("Security").Element("Signature").Value =
                signer.SignData(request.Element("MsgBody").ToString());

            var service = new PaymentInquiryClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.PaymentInquryUrl));
            var response = service.Inquire(guid, token, request);

            Audit("PaymentInquiryRequest response:" + response);

            if (response.Element("MsgBody") != null)
            {                
                trxInf = response.Element("MsgBody").Element("Transactions").Element("TrxInf");
                result.Error = Convert.ToInt32(trxInf.Element("Result").Element("ErrorCode").Value);
            }
            else
                result.Error = Convert.ToInt32(response.Element("MsgHeader").Element("Result").Element("ErrorCode").Value);

            result.StmtDate = DateTime.Now;

            return result;
        }

        public override string ProcessOnlineCheck(NewPaymentData paymentData, object operatorData)
        {
            int responseResult = 0;
            string param = string.Empty;
            var paymentResult = new PaymentResult();
            try
            {
                var operatorRow = operatorData as DataRow;
                if (operatorRow == null)
                    throw new Exception("unable to extract paymentData");

                var operatorFormatString = operatorRow["OsmpFormatString"] is DBNull
                    ? ""
                    : operatorRow["OsmpFormatString"] as string;
                var formatedPaymentParams = paymentData.Params.FormatParameters(operatorFormatString);
                var parametersList = new StringList(formatedPaymentParams, ";");

                var paymentType = (PaymentType) Enum.Parse(typeof (PaymentType), parametersList.Get("PymentType"));                
                switch (paymentType)
                {
                    case PaymentType.Prepaid:
                        paymentResult = PrepaidValidationRequest(paymentData.CyberplatOperatorID, parametersList);
                        if(paymentResult.Error == 0)
                            param = string.Format("DUEAMOUNT={0}\r\nLOWERAMOUNT=0\r\nUPPERAMOUNT=0", paymentResult.Params.Get("DueAmt"));
                        break;
                    case PaymentType.Postpaid:
                        paymentResult = BillInquiryRequest(paymentData.CyberplatOperatorID, parametersList);
                        if (paymentResult.Error == 0)
                            param = string.Format("DUEAMOUNT={0}\r\nLOWERAMOUNT={1}\r\nUPPERAMOUNT={2}", paymentResult.Params.Get("DueAmt"), paymentResult.Params.Get("LOWERAMOUNT"), paymentResult.Params.Get("UPPERAMOUNT"));
                        break;
                }
            }
            catch (Exception ex)
            {
                Audit("ProcessOnlineCheck exception: " + ex.Message);
                responseResult = 30;
            }

            if(paymentResult.Error != 0)
                responseResult = 30;

            string responseString = "DATE=" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "\r\n" + "SESSION=" +
                                    paymentData.Session + "\r\n" + "ERROR=" + responseResult + "\r\n" + "RESULT=" +
                                    ((responseResult == 0) ? "0" : "1") + "\r\n" + param + "\r\n";

            return responseString;
        }

        public string CheckSettings()
        {
            /*
             * проверяется только успешность получения токена, остальные службы не проверяются.
             * они должны работать в комплексе, и если нет доступа к этой - то работоспособность 
             * остальных не имеет особого значения
             */
            var message = string.Empty;
            try
            {
                Authenticate();

                message = "OK";
            }
            catch (Exception ex)
            {
                message += " (Exception): " + ex.Message;
            }

            return message;
        }

        public IGateway Clone()
        {
            return new EfawateerGateway(this);
        }

        private string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string Authenticate()
        {

            if (!AuthenticateTokenProvider.Current.IsExpired)
                return AuthenticateTokenProvider.Current.TokenKey;

            if (_tokenGuid == Guid.Empty)
                _tokenGuid = Guid.NewGuid();

            var token = new TokenServiceClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.TokenUrl));
            var authenticate = token.Authenticate(_tokenGuid.ToString(), Convert.ToInt32(_config.CustomerCode), _config.Password);

            Audit("Authenticate" + authenticate);

            var body = authenticate.Element("MsgBody");
            if (body == null)
                AuthenticateTokenProvider.Current =
                    new ErrorToken(authenticate.Element("MsgHeader").Element("Result").ToString());
            else
            {
                var expdate = body.Element("TokenConf").Element("ExpiryDate").Value;
                var key = body.Element("TokenConf").Element("TokenKey").Value;
                AuthenticateTokenProvider.Current = new SuccessToken(expdate, key);
            }

            return AuthenticateTokenProvider.Current.TokenKey;
        }

        private XElement GetRequestContent(string request)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (
                var stream = assembly.GetManifestResourceStream(string.Format("EfawateerGateway.Proxy.{0}.xml", request))
                )
            using (var reader = new StreamReader(stream))
                return XElement.Parse(reader.ReadToEnd());
        }

        private int EfawateerCodeToCyberCode(int code)
        {
            if (code == 0)
                return 0;
            return 10000 + code;
        }

        public int ExpandBillerCodeFromCyberplatOpertaroId(int cyberplatOpertaroId)
        {
            return cyberplatOpertaroId%1000;
        }

        public PaymentResult PrepaidValidationRequest(int cyberplatOperatorId, StringList parametersList)
        {
            var billerCode = ExpandBillerCodeFromCyberplatOpertaroId(cyberplatOperatorId);

            var token = Authenticate();
            var request = GetRequestContent(Prepadvalrq);
            var signer = new EfawateerSigner(_config.SignCertificateThumb, _config.VerifyCertificateThumb);

            var time = DateTime.Now.ToString("s");
            var guid = GenerateGuid();
            request.Element("MsgHeader").Element("TmStp").Value = time;
            request.Element("MsgHeader").Element("TrsInf").Element("SdrCode").Value = _config.CustomerCode;
            request.Element("MsgHeader").Element("GUID").Value = guid;

            var billInfo = request.Element("MsgBody").Element("BillingInfo");
            var accInfo = billInfo.Element("AcctInfo");
            accInfo.Element("BillerCode").Value = billerCode.ToString();

            if (!parametersList.ContainsKey("BillingNo") || parametersList["BillingNo"].Contains("#"))
                accInfo.Element("BillingNo").Remove();
            else
                accInfo.Element("BillingNo").Value = parametersList["BillingNo"];

            var serviceTypeDetails = billInfo.Element("ServiceTypeDetails");

            serviceTypeDetails.Element("ServiceType").Value = parametersList["ServiceType"];

            if (!parametersList.ContainsKey("PrepaidCat"))
                serviceTypeDetails.Element("PrepaidCat").Remove();
            else
                serviceTypeDetails.Element("PrepaidCat").Value = parametersList["PrepaidCat"];

            if (!parametersList.ContainsKey("DueAmt"))
                billInfo.Element("DueAmt").Remove();
            else
                billInfo.Element("DueAmt").Value = parametersList["DueAmt"];


            Audit("Validation request:" + request);

            request.Element("MsgFooter").Element("Security").Element("Signature").Value =
                signer.SignData(request.Element("MsgBody").ToString());

            var service = new PrepaidValidationClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.ValidationUrl));
            var response = service.Validate(guid, token, request);

            Audit("Validation response:" + response);

            billInfo = response.Element("MsgBody").Element("BillingInfo");

            var errorCode = Convert.ToInt32(billInfo.Element("Result").Element("ErrorCode").Value);

            if (errorCode == 0)
            {
                var validationCode = billInfo.Element("ValidationCode").Value;

                if (parametersList.ContainsKey("ValidationCode"))
                    parametersList.Remove("ValidationCode");

                parametersList.Add("ValidationCode", validationCode);
            }

            return new PaymentResult
            {
                Params = parametersList,
                Error = errorCode
            };
        }

        public PaymentResult PrepaidPaymentRequest(int cyberplatOperatorId, StringList parametersList, string session)
        {

            var result = new PaymentResult
            {
                Error = 0
            };

            var billerCode = ExpandBillerCodeFromCyberplatOpertaroId(cyberplatOperatorId);

            var token = Authenticate();
            var request = GetRequestContent(Prepadpmtrq);
            var signer = new EfawateerSigner(_config.SignCertificateThumb, _config.VerifyCertificateThumb);

            var now = DateTime.Now;
            var time = now.ToString("s");
            var guid = GenerateGuid();

            request.Element("MsgHeader").Element("TmStp").Value = time;
            request.Element("MsgHeader").Element("TrsInf").Element("SdrCode").Value = _config.CustomerCode;
            request.Element("MsgHeader").Element("GUID").Value = guid;

            var trxInf = request.Element("MsgBody").Element("TrxInf");
            var accInfo = trxInf.Element("AcctInfo");

            if (!parametersList.ContainsKey("BillingNo") || parametersList["BillingNo"].Contains("#"))
                accInfo.Element("BillingNo").Remove();
            else
                accInfo.Element("BillingNo").Value = parametersList["BillingNo"];

            accInfo.Element("BillerCode").Value = billerCode.ToString();

            trxInf.Element("ServiceTypeDetails").Element("ServiceType").Value = parametersList.Get("ServiceType");

            trxInf.Element("DueAmt").Value = parametersList.Get("DueAmt");
            trxInf.Element("PaidAmt").Value = parametersList.Get("DueAmt");
            trxInf.Element("ValidationCode").Value = parametersList.Get("ValidationCode");
            trxInf.Element("ProcessDate").Value = parametersList.Get("InitializeDateTime");
            trxInf.Element("BankTrxID").Value = session;

            Audit("PrepaidPaymentRequest request:" + request);

            request.Element("MsgFooter").Element("Security").Element("Signature").Value =
                signer.SignData(request.Element("MsgBody").ToString());

            var service = new PrepaidPaymentClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.PrepaidUrl));
            var response = service.Pay(guid, token, request);

            Audit("PrepaidPaymentRequest response:" + response);

            trxInf = response.Element("MsgBody").Element("TrxInf");
            result.JoebppsTrx = trxInf.Element("JOEBPPSTrx").Value;
            result.StmtDate = DateTime.Parse(trxInf.Element("STMTDate").Value);
            result.Error = Convert.ToInt32(trxInf.Element("Result").Element("ErrorCode").Value);
            result.Params = parametersList;

            return result;
        }

        public PaymentResult BillInquiryRequest(int cyberplatOperatorId, StringList parametersList)
        {
            var billerCode = ExpandBillerCodeFromCyberplatOpertaroId(cyberplatOperatorId);

            var token = Authenticate();
            var request = GetRequestContent(Bilinqrq);
            var signer = new EfawateerSigner(_config.SignCertificateThumb, _config.VerifyCertificateThumb);

            var now = DateTime.Now;

            var time = now.ToString("s");
            var guid = GenerateGuid();
            request.Element("MsgHeader").Element("TmStp").Value = time;
            request.Element("MsgHeader").Element("TrsInf").Element("SdrCode").Value = _config.CustomerCode;

            var billInfo = request.Element("MsgBody");
            var accInfo = billInfo.Element("AcctInfo");
            accInfo.Element("BillerCode").Value = billerCode.ToString(CultureInfo.InvariantCulture);

            if (!parametersList.ContainsKey("BillingNo") || parametersList["BillingNo"].Contains("#"))
                accInfo.Element("BillingNo").Remove();
            else
                accInfo.Element("BillingNo").Value = parametersList["BillingNo"];

            billInfo.Element("ServiceType").Value = parametersList.Get("ServiceType");

            var dateRange = billInfo.Element("DateRange");
            dateRange.Element("StartDt").Value = now.AddDays(-_config.StartDt).ToString("s");
            dateRange.Element("EndDt").Value = time;

            Audit("BillInquiryRequest request:" + request);

            request.Element("MsgFooter").Element("Security").Element("Signature").Value =
                signer.SignData(request.Element("MsgBody").ToString());

            var service = new BillInquiryClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.InquiryUrl));
            var response = service.Inquire(guid, token, request);

            Audit("BillInquiryRequest response:" + response);

            var errorCode = Convert.ToInt32(response.Element("MsgHeader").Element("Result").Element("ErrorCode").Value);

            if (errorCode != 0)
                return new PaymentResult
                {
                    Params = parametersList,
                    Error = errorCode
                };

            errorCode = 0;

            if (response.Element("MsgBody") == null)
                errorCode = 10;
            else if (response.Element("MsgBody").Element("BillsRec") == null)
                errorCode = 11;
            else
            {
                var billRec = response.Element("MsgBody").Element("BillsRec").Element("BillRec");

                if (billRec.Element("OpenDate") != null)
                {
                    var openDate = DateTime.Parse(billRec.Element("OpenDate").Value);
                    if (openDate > now)
                        errorCode = 12;
                    //throw new Exception("Невозможно выполнить оплату счет в будущем (OpenDate)");
                }
                else
                {
                    var dueDate = DateTime.Parse(billRec.Element("DueDate").Value);
                    if (dueDate > now)
                        errorCode = 13;
                    //throw new Exception("Невозможно выполнить оплату счет в будущем (DueDate)");
                }

                if (billRec.Element("ExpiryDate") != null)
                {
                    var expiryDate = DateTime.Parse(billRec.Element("ExpiryDate").Value);
                    if (expiryDate < now)
                        errorCode = 14;
                    //throw new Exception("Невозможно выполнить оплату счет в прошлом (ExpiryDate)");
                }

                if (billRec.Element("CloseDate") != null)
                {
                    var closeDate = DateTime.Parse(billRec.Element("CloseDate").Value);
                    if (closeDate < now)
                        errorCode = 15;
                    //throw new Exception("Невозможно выполнить оплату счет в прошлом (CloseDate)");
                }

                if (errorCode == 0)
                {

                    var dueAmt = billRec.Element("DueAmount").Value;
                    var inqRefNo = string.Empty;
                    if (billRec.Element("InqRefNo") != null)
                        inqRefNo = billRec.Element("InqRefNo").Value;

                    var pmtConst = billRec.Element("PmtConst");
                    var lower = pmtConst.Element("Lower").Value;
                    var upper = pmtConst.Element("Upper").Value;
                    var allowPart = pmtConst.Element("AllowPart").Value;

                    parametersList.Add("INQREFNO", inqRefNo);
                    if (parametersList.ContainsKey("DueAmt"))
                        parametersList.Remove("DueAmt");
                    parametersList.Add("DueAmt", dueAmt);
                    parametersList.Add("AllowPart", allowPart);
                    parametersList.Add("LOWERAMOUNT", lower);
                    parametersList.Add("UPPERAMOUNT", upper);
                }
            }            

            return new PaymentResult
            {
                Params = parametersList,
                Error = errorCode
            };
        }

        public PaymentResult BillPaymentRequest(int cyberplatOperatorId, StringList parametersList, string session)
        {
            var result = new PaymentResult
            {
                Error = 0
            };

            if (parametersList.ContainsKey("AllowPart") && bool.Parse(parametersList["AllowPart"]))
            {
                // todo проверка LOWER/UPPER
            }

            var token = Authenticate();
            var request = GetRequestContent(Bilpmtrq);
            var signer = new EfawateerSigner(_config.SignCertificateThumb, _config.VerifyCertificateThumb);

            var guid = GenerateGuid();
            var time = DateTime.Now.ToString("s");

            request.Element("MsgHeader").Element("TmStp").Value = time;
            request.Element("MsgHeader").Element("TrsInf").Element("SdrCode").Value = _config.CustomerCode;

            var trxInf = request.Element("MsgBody").Element("Transactions").Element("TrxInf");

            var accInfo = trxInf.Element("AcctInfo");

            if (!parametersList.ContainsKey("BillingNo") || parametersList["BillingNo"].Contains("#"))
            {
                accInfo.Element("BillingNo").Remove();
                accInfo.Element("BillNo").Remove();
            }
            else
            {
                accInfo.Element("BillingNo").Value = parametersList["BillingNo"];
                accInfo.Element("BillNo").Value = parametersList["BillingNo"];
            }

            accInfo.Element("BillerCode").Value =
                ExpandBillerCodeFromCyberplatOpertaroId(cyberplatOperatorId).ToString(CultureInfo.InvariantCulture);

            trxInf.Element("ServiceTypeDetails").Element("ServiceType").Value = parametersList.Get("ServiceType");

            trxInf.Element("DueAmt").Value = parametersList.Get("DueAmt");
            trxInf.Element("PaidAmt").Value = parametersList.Get("DueAmt");

            trxInf.Element("ProcessDate").Value = parametersList.Get("InitializeDateTime");
            trxInf.Element("BankTrxID").Value = session;

            Audit("BillPaymentRequest request:" + request);

            request.Element("MsgFooter").Element("Security").Element("Signature").Value =
                signer.SignData(request.Element("MsgBody").ToString());

            var service = new PaymentClient(new WSHttpBinding(SecurityMode.None, true)
            {
                ReceiveTimeout = _config.Timeout
            }, new EndpointAddress(_config.PaymentUrl));
            var response = service.PayBill(guid, token, request);

            Audit("BillPaymentRequest response:" + response);

            trxInf = response.Element("MsgBody").Element("Transactions").Element("TrxInf");
            result.Error = Convert.ToInt32(trxInf.Element("Result").Element("ErrorCode").Value);

            if (trxInf.Element("JOEBPPSTrx") != null)
                result.JoebppsTrx = trxInf.Element("JOEBPPSTrx").Value;

            if (trxInf.Element("STMTDate") != null)
                result.StmtDate = DateTime.Parse(trxInf.Element("STMTDate").Value);

            result.Params = parametersList;

            return result;
        }

        private void Audit(string message)
        {
#if !TEST
            if (_config.DetailedLog)
                log(message);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public string GetData(string request, string parameters)
        {
            try
            {
                var token = Authenticate();

                var client = new BillersListClient(new WSHttpBinding(SecurityMode.None, true)
                {
                    ReceiveTimeout = _config.Timeout
                }, new EndpointAddress(_config.BillerList));
                var list = client.GetBillersList(Guid.NewGuid().ToString(), token);
                return list.ToString();
            }
            catch (Exception exception)
            {
                return string.Format("Ошибка получения данных: " + exception.Message);
            }
        }
    }
    
}