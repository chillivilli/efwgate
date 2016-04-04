namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "PrepaidValidation.IPrepaidValidation")]
    public interface IPrepaidValidation
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IPrepaidValidation/Validate", ReplyAction = "http://tempuri.org/IPrepaidValidation/ValidateResponse")]
        System.Xml.Linq.XElement Validate(string guid, string tokenKey, System.Xml.Linq.XElement paymnentValidationRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPrepaidValidationChannel : IPrepaidValidation, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PrepaidValidationClient : System.ServiceModel.ClientBase<IPrepaidValidation>, IPrepaidValidation
    {

        public PrepaidValidationClient()
        {
        }

        public PrepaidValidationClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public PrepaidValidationClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PrepaidValidationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PrepaidValidationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement Validate(string guid, string tokenKey, System.Xml.Linq.XElement paymnentValidationRequest)
        {
            return base.Channel.Validate(guid, tokenKey, paymnentValidationRequest);
        }
    }
}
