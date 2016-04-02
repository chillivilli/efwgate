using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace EfawateerGateway
{
    class GatewayConfig : ICloneable
    {
        public GatewayConfig() { }
        
        private bool _detailLogEnabled;

        private string _customerCode;
        private string _password;
        private string _signCertificate;

        private string _verifyCertificate;

        private string _tokenUrl;
        private string _inquiryUrl;
        private string _paymentUrl;
        private string _prepaidUrl;
        private string _validationUrl;
        private string _paymentInquryUrl;
        private string _billerList;

        private Int32 _timeout;
        private Int32 _startdt;


        public bool DetailedLog { get { return _detailLogEnabled; } }

        public string CustomerCode { get { return _customerCode; } }

        public string Password { get { return _password; } }

        public string SignCertificateThumb { get { return _signCertificate; } }

        public string VerifyCertificateThumb { get { return _verifyCertificate; } }

        public string TokenUrl { get { return _tokenUrl; } }

        public string InquiryUrl { get { return _inquiryUrl; } }

        public string PaymentUrl { get { return _paymentUrl; } }

        public string PrepaidUrl { get { return _prepaidUrl; } }

        public string ValidationUrl { get { return _validationUrl; } }

        public string PaymentInquryUrl { get { return _paymentInquryUrl; } }

        public string BillerList { get { return _billerList; } }

        public TimeSpan Timeout { get { return TimeSpan.FromMilliseconds(_timeout); } }

        public Int32 StartDt { get { return _startdt; } }

        public void Load(string data)
        {
            var xmlData = new XmlDocument();
            xmlData.LoadXml(data);

            _tokenUrl = xmlData.DocumentElement["token_url"].InnerText;
            _inquiryUrl = xmlData.DocumentElement["inquiry_url"].InnerText;
            _paymentUrl = xmlData.DocumentElement["payment_url"].InnerText;
            _prepaidUrl = xmlData.DocumentElement["prepaid_payment_url"].InnerText;
            _validationUrl = xmlData.DocumentElement["prepare_validation_url"].InnerText;
            _paymentInquryUrl = xmlData.DocumentElement["payment_inqury_url"].InnerText;
            _billerList = xmlData.DocumentElement["biller_list_url"].InnerText;

            _customerCode = xmlData.DocumentElement["customer_code"].InnerText;
            _password = xmlData.DocumentElement["password"].InnerText;

            _signCertificate = xmlData.DocumentElement["sign_crt"].InnerText;

            _verifyCertificate = xmlData.DocumentElement["verify_crt"].InnerText;
             
            _timeout = Convert.ToInt32(xmlData.DocumentElement["timeout"].InnerText);
            _startdt = Convert.ToInt32(xmlData.DocumentElement["startdt"].InnerText);

            if (xmlData.DocumentElement["detail_log"] != null &&
                (xmlData.DocumentElement["detail_log"].InnerText.ToLower() == "true" ||
                 xmlData.DocumentElement["detail_log"].InnerText == "1"))
            {
                _detailLogEnabled = true;
            }
            else
                _detailLogEnabled = false;
        }


   

        public object Clone()
        {
            GatewayConfig clone = new GatewayConfig();
            clone._billerList = _billerList;
            clone._signCertificate = _signCertificate;
            clone._verifyCertificate = _verifyCertificate;
            clone._customerCode = _customerCode;
            clone._detailLogEnabled = _detailLogEnabled;
            clone._inquiryUrl = _inquiryUrl;
            clone._password = _password;
            clone._paymentInquryUrl = _paymentInquryUrl;
            clone._paymentUrl = _paymentUrl;
            clone._prepaidUrl = _prepaidUrl;
            clone._startdt = _startdt;
            clone._timeout = _timeout;
            clone._tokenUrl = _tokenUrl;
            clone._validationUrl = _validationUrl;

            return clone;
        }

    }
}
