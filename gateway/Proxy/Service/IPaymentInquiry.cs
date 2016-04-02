namespace EfawateerGateway.Proxy.Service
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "PaymentInquiry.IPaymentInquiry")]
    public interface IPaymentInquiry
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IPaymentInquiry/Inquire", ReplyAction = "http://tempuri.org/IPaymentInquiry/InquireResponse")]
        System.Xml.Linq.XElement Inquire(string guid, string tokenKey, System.Xml.Linq.XElement paymentInquireRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPaymentInquiryChannel : IPaymentInquiry, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PaymentInquiryClient : System.ServiceModel.ClientBase<IPaymentInquiry>, IPaymentInquiry
    {

        public PaymentInquiryClient()
        {
        }

        public PaymentInquiryClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public PaymentInquiryClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PaymentInquiryClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PaymentInquiryClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement Inquire(string guid, string tokenKey, System.Xml.Linq.XElement paymentInquireRequest)
        {
            return base.Channel.Inquire(guid, tokenKey, paymentInquireRequest);
        }
    }
}
