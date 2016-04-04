namespace EfawateerTests.Proxy
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "BillInquiry.IBillInquiry")]
    public interface IBillInquiry
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBillInquiry/Inquire", ReplyAction = "http://tempuri.org/IBillInquiry/InquireResponse")]
        System.Xml.Linq.XElement Inquire(string guid, string tokenKey, System.Xml.Linq.XElement billInquiryRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBillInquiryChannel : IBillInquiry, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BillInquiryClient : System.ServiceModel.ClientBase<IBillInquiry>, IBillInquiry
    {

        public BillInquiryClient()
        {
        }

        public BillInquiryClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public BillInquiryClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillInquiryClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillInquiryClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement Inquire(string guid, string tokenKey, System.Xml.Linq.XElement billInquiryRequest)
        {
            return base.Channel.Inquire(guid, tokenKey, billInquiryRequest);
        }
    }
}
