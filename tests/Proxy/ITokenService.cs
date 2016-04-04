using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "Token.ITokenService")]
    public interface ITokenService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ITokenService/Authenticate", ReplyAction = "http://tempuri.org/ITokenService/AuthenticateResponse")]
        System.Xml.Linq.XElement Authenticate(string guid, int customerCode, string password);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ITokenService/ValidateToken", ReplyAction = "http://tempuri.org/ITokenService/ValidateTokenResponse")]
        bool ValidateToken(int customerCode, string password, string token);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITokenServiceChannel : ITokenService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TokenServiceClient : System.ServiceModel.ClientBase<ITokenService>, ITokenService
    {

        public TokenServiceClient()
        {
        }

        public TokenServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public TokenServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public TokenServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public TokenServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement Authenticate(string guid, int customerCode, string password)
        {
            return base.Channel.Authenticate(guid, customerCode, password);
        }

        public bool ValidateToken(int customerCode, string password, string token)
        {
            return base.Channel.ValidateToken(customerCode, password, token);
        }
    }
}
