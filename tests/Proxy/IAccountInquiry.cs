using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "AccountInquiry.IAccountInquiry")]
    public interface IAccountInquiry
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAccountInquiry/AccountInquiry", ReplyAction = "http://tempuri.org/IAccountInquiry/AccountInquiryResponse")]
        System.Xml.Linq.XElement AccountInquiry(string guid, string tokenKey, System.Xml.Linq.XElement accountInquiryRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAccountInquiryChannel : IAccountInquiry, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AccountInquiryClient : System.ServiceModel.ClientBase<IAccountInquiry>, IAccountInquiry
    {

        public AccountInquiryClient()
        {
        }

        public AccountInquiryClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AccountInquiryClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AccountInquiryClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AccountInquiryClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement AccountInquiry(string guid, string tokenKey, System.Xml.Linq.XElement accountInquiryRequest)
        {
            return base.Channel.AccountInquiry(guid, tokenKey, accountInquiryRequest);
        }
    }
}
