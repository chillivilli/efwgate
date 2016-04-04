using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "AddCustomerBilling.IAddCustomerBilling")]
    public interface IAddCustomerBilling
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAddCustomerBilling/AddCustomerBilling", ReplyAction = "http://tempuri.org/IAddCustomerBilling/AddCustomerBillingResponse")]
        System.Xml.Linq.XElement AddCustomerBilling(string guid, string tokenKey, System.Xml.Linq.XElement addCustomerBillingRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAddCustomerBillingChannel : IAddCustomerBilling, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AddCustomerBillingClient : System.ServiceModel.ClientBase<IAddCustomerBilling>, IAddCustomerBilling
    {

        public AddCustomerBillingClient()
        {
        }

        public AddCustomerBillingClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AddCustomerBillingClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AddCustomerBillingClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AddCustomerBillingClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement AddCustomerBilling(string guid, string tokenKey, System.Xml.Linq.XElement addCustomerBillingRequest)
        {
            return base.Channel.AddCustomerBilling(guid, tokenKey, addCustomerBillingRequest);
        }
    }
}
