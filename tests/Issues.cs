using Gateways.Utils;
using EfawateerGateway;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests
{
    [TestClass]
    public class Issues
    {
        [TestMethod]
        [TestCategory("Issues")]
        public void PaymentTypeNotFoundInStringList()
        {
            string paramString = "PaymentType=Postpaid;BillingNo=1112;ServiceType=Internet;DueAmt=[#AMOUNT]";
            StringList list = new StringList(paramString, ";");
            Assert.AreEqual("Postpaid", list.GetParam("PaymentType"));
        }

        [TestMethod]
        [TestCategory("Issues")]
        public void FormatParamsWithoutTokens()
        {
            string paramString = "PAYMENTTYPE=Postpaid\nNUMBER=1112\nSERVICETYPE=Internet\nAMOUNT=[#AMOUNT]";
            string result = paramString.FormatParameters("PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#AMOUNT]");
            Assert.AreEqual("PaymentType=Postpaid;BillingNo=1112;ServiceType=Internet", result);
        }


        [TestMethod]
        [TestCategory("Issues")]
        public void FormatParamsException()
        {
            string paramString = "PAYMENTAMOUNT=15\nSERVICETYPE=Prepaid\nPAYMENTTYPE=Prepaid\n";
            string result = paramString.FormatParameters("PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#PAYMENTAMOUNT]");
            Assert.AreEqual("PaymentType=Postpaid;BillingNo=1112;ServiceType=Internet", result);
        }

        [TestMethod()]
        [TestCategory("Issues")]
        public void FormatParamsLoosAmount()
        {
            string paramString = "PaymentType=Prepaid\nBillingNo=[#NUMBER]\nServiceType=Prepaid\nDueAmt=15\nInitializeDateTime=02.04.2016 23:17:08\nSession=dc437f47-a7da-4d8e-be14-0d6909dd8020\n";
            string opString = "PaymentType=[#PAYMENTTYPE];BillingNo=[#NUMBER];ServiceType=[#SERVICETYPE];DueAmt=[#PAYMENTAMOUNT]";
            string result = paramString.FormatParameters(opString);
            Assert.AreEqual(string.Empty, result);

        }

        
        
    }
}
