using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Gateways;
using Gateways.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EfawateerGateway;

namespace EfawateerTests
{
    [TestClass]
    public class process
    {
        private DataTable _paymentTable;
        private DataTable _operatorTable;

        [TestInitialize]
        public void Initialize()
        {
            _paymentTable = new DataTable();
            _paymentTable.Columns.Add(new DataColumn("InitialSessionNumber", typeof (string)));
            _paymentTable.Columns.Add(new DataColumn("SessionNumber", typeof(string)));
            _paymentTable.Columns.Add(new DataColumn("TerminalID", typeof(int)));
            _paymentTable.Columns.Add(new DataColumn("StatusID", typeof(int)));
            _paymentTable.Columns.Add(new DataColumn("ErrorCode", typeof(int)));
            _paymentTable.Columns.Add(new DataColumn("Params", typeof(string)));
            _paymentTable.Columns.Add(new DataColumn("Amount", typeof(double)));
            _paymentTable.Columns.Add(new DataColumn("AmountAll", typeof(double)));
            _paymentTable.Columns.Add(new DataColumn("CyberplatOperatorID", typeof(int)));

            _operatorTable = new DataTable();
            _operatorTable.Columns.Add(new DataColumn("OsmpFormatString", typeof(string)));
        }

        [TestMethod, Description("Ручная проверка")]
        public void prepaid_full_circle()
        {
            var paymentTbl = _paymentTable.NewRow();
            paymentTbl["TerminalID"] = 100000;
            paymentTbl["StatusID"] = 0;
            paymentTbl["ErrorCode"] = 0;
            paymentTbl["Params"] = "AMOUNT=51\nSERVICETYPE=Prepaid\nPAYMENTTYPE=Prepaid\n";
            paymentTbl["Amount"] = 3;
            paymentTbl["AmountAll"] = 3;
            paymentTbl["CyberplatOperatorID"] = 70039;

            var operatorTbl = _operatorTable.NewRow();
            operatorTbl["OsmpFormatString"] = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#AMOUNT]";

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));
            gate.ProcessPayment(paymentTbl, operatorTbl, null);
        }

        [TestMethod]
        public void check_status_prepaid()
        {
            var paymentTbl = _paymentTable.NewRow();
            paymentTbl["TerminalID"] = 100000;
            paymentTbl["StatusID"] = 6;
            paymentTbl["ErrorCode"] = 0;
            paymentTbl["Params"] = "PaymentType=Prepaid;BillingNo=9050010203;ServiceType=Test_Prepaid;DueAmt=5;ValidationCode=76392";
            paymentTbl["Amount"] = 3;
            paymentTbl["AmountAll"] = 3;
            paymentTbl["CyberplatOperatorID"] = 70039;
            paymentTbl["SessionNumber"] = "5541b12d-07db-4ef2-b075-40433a77187f";

            var operatorTbl = _operatorTable.NewRow();
            operatorTbl["OsmpFormatString"] = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#AMOUNT]";

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));
            gate.ProcessPayment(paymentTbl, operatorTbl, null);
        }

        [TestMethod, Description("Ручная проверка")]
        public void postpaid_full_circle()
        {
            var paymentTbl = _paymentTable.NewRow();
            paymentTbl["TerminalID"] = 100000;
            paymentTbl["StatusID"] = 0;
            paymentTbl["ErrorCode"] = 0;
            paymentTbl["Params"] = "PAYMENTTYPE=Postpaid\nNUMBER=9050010203\nSERVICETYPE=Electricity";
            paymentTbl["Amount"] = 3;
            paymentTbl["AmountAll"] = 3;
            paymentTbl["CyberplatOperatorID"] = 70039;

            var operatorTbl = _operatorTable.NewRow();
            operatorTbl["OsmpFormatString"] = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#AMOUNT]";

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));
            gate.ProcessPayment(paymentTbl, operatorTbl, null);
        }

        [TestMethod]
        public void check_status_postpaid()
        {
            var paymentTbl = _paymentTable.NewRow();
            paymentTbl["TerminalID"] = 100000;
            paymentTbl["StatusID"] = 6;
            paymentTbl["ErrorCode"] = 0;
            paymentTbl["Params"] = "PaymentType=Postpaid;BillingNo=9050010203;ServiceType=Electricity;DueAmt=8.5;INQREFNO=;AllowPart=true;LOWERAMOUNT=1.50;UPPERAMOUNT=2000.50;JoebppsTrx=2016030915821204";
            paymentTbl["Amount"] = 3;
            paymentTbl["AmountAll"] = 3;
            paymentTbl["CyberplatOperatorID"] = 70039;
            paymentTbl["SessionNumber"] = "c3936677-62db-400d-b8f8-2de1768a2288";

            var operatorTbl = _operatorTable.NewRow();
            operatorTbl["OsmpFormatString"] = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#AMOUNT]";

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));
            gate.ProcessPayment(paymentTbl, operatorTbl, null);
        }

        [TestMethod, Description("Ручная проверка")]
        public void ProcessOnlineCheck()
        {
            var data = new NewPaymentData
            {
                Params = "billingno=25\\nbillingcode=39"
            };

            var row = _operatorTable.NewRow();
            row["OsmpFormatString"] = "billingno=[#billingno];billingcode=[#billingcode];";

            var gate = new Gateways.EfawateerGateway();

            gate.Initialize(File.ReadAllText("initialize.xml"));

            var processOnlineCheck = gate.ProcessOnlineCheck(data, row);
        }

        [TestMethod, Description("Ручная проверка")]
        public void PostPayment()
        {
            var data = new NewPaymentData
            {
                Params = "billingno=25\\nbillingcode=39"
            };

            var row = _operatorTable.NewRow();
            row["OsmpFormatString"] = "billingno=[#billingno];billingcode=[#billingcode];";

            var gate = new Gateways.EfawateerGateway();

            gate.Initialize(File.ReadAllText("initialize.xml"));

            var processOnlineCheck = gate.ProcessOnlineCheck(data, row);

            var q = processOnlineCheck.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries).First(s => s.StartsWith("DUE")).Replace("DUE=", string.Empty);

            var paymentTbl = _paymentTable.NewRow();
            paymentTbl["TerminalID"] = 10;
            paymentTbl["StatusID"] = 1;
            paymentTbl["ErrorCode"] = 0;
            paymentTbl["Params"] = "billingno=25\\nbillingcode=39";            
            paymentTbl["Amount"] = Convert.ToDouble(q, CultureInfo.InvariantCulture);
            paymentTbl["AmountAll"] = Convert.ToDouble(q, CultureInfo.InvariantCulture);

            var operatorTbl = _operatorTable.NewRow();
            operatorTbl["OsmpFormatString"] = "billingno=[#billingno];billingcode=[#billingcode];";
            
            gate.ProcessPayment(paymentTbl, operatorTbl, null);
        }

        [TestMethod]
        public void prepaid_validation_service_type_request()
        {
            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));

            var list = new StringList("ServiceType=Prepaid;BillingNo=[#NUMBER];DueAmt=43.12", ";");

            var request = gate.PrepaidValidationRequest(700162, list);
        }

        [TestMethod]
        public void bill_inquiry_request()
        {
            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));

            var list = new StringList("BillingNo=9010020304;ServiceType=Electricity", ";");

            var request = gate.BillInquiryRequest(700039, list);
        }

        [TestMethod]
        public void bill_payment_request()
        {

            var session = Guid.NewGuid().ToString();

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));

            var list = new StringList("BillingNo=9010020304;ServiceType=Electricity", ";");

            var request = gate.BillInquiryRequest(700039, list);
            var dueAmt = request.Params["DueAmt"];

            list = new StringList(string.Format("BillingNo=9010020304;ServiceType=Electricity;DueAmt={0}", dueAmt), ";");

            var response = gate.BillPaymentRequest(700039, list, session, DateTime.Parse("2014-02-22"), "123");

            Assert.AreEqual(0, response.Error);
        }

        [TestMethod]
        public void prepaid_payment_request()
        {
            var session = Guid.NewGuid().ToString();

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));

            var list = new StringList("BillingNo=9010020304;ServiceType=Prepaid;DueAmt=43.12;ValidationCode=1234567", ";");
            var response = gate.PrepaidPaymentRequest(700162, list, session, DateTime.Parse("2014-02-22"), "123");

            Assert.AreEqual(0, response.Error);
        }

        [TestMethod]
        public void payment_inquiry_request()
        {
            var session = Guid.NewGuid().ToString();

            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));

            var list = new StringList("BillingNo=9010020304;ServiceType=Electricity;DueAmt=43.12;ValidationCode=1234567;PaymentType=Prepaid", ";");
            var response = gate.PaymentInquiryRequest(700039, list, session, DateTime.Parse("2014-02-22"), "123");

            Assert.AreEqual(0, response.Error);
        }

        [TestMethod]
        public void get_data()
        {
            var gate = new Gateways.EfawateerGateway();
            gate.Initialize(File.ReadAllText("initialize.xml"));
            var data = gate.GetData("", "");
        }

        [TestMethod]
        public void foo()
        {
            var format = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE]";
            var p = "AMOUNT=15\nSERVICETYPE=Prepaid\nPAYMENTTYPE=Prepaid\n";
            var formatParameters = p.FormatParameters(format);
        }
    }
}
