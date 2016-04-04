using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "BillersList.IBillersList")]
    public interface IBillersList
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBillersList/GetBillersList", ReplyAction = "http://tempuri.org/IBillersList/GetBillersListResponse")]
        System.Xml.Linq.XElement GetBillersList(string guid, string tokenKey);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBillersListChannel : IBillersList, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BillersListClient : System.ServiceModel.ClientBase<IBillersList>, IBillersList
    {

        public BillersListClient()
        {
        }

        public BillersListClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public BillersListClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillersListClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillersListClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement GetBillersList(string guid, string tokenKey)
        {
            return base.Channel.GetBillersList(guid, tokenKey);
        }
    }
}
