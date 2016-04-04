using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "AccountUpload.IAccountUpload")]
    public interface IAccountUpload
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAccountUpload/AccountUpload", ReplyAction = "http://tempuri.org/IAccountUpload/AccountUploadResponse")]
        System.Xml.Linq.XElement AccountUpload(string guid, string tokenKey, System.Xml.Linq.XElement accountUploadRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAccountUploadChannel : IAccountUpload, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AccountUploadClient : System.ServiceModel.ClientBase<IAccountUpload>, IAccountUpload
    {

        public AccountUploadClient()
        {
        }

        public AccountUploadClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AccountUploadClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AccountUploadClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AccountUploadClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement AccountUpload(string guid, string tokenKey, System.Xml.Linq.XElement accountUploadRequest)
        {
            return base.Channel.AccountUpload(guid, tokenKey, accountUploadRequest);
        }
    }
}
