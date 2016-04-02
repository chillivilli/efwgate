namespace EfawateerGateway.Proxy.Service
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "BillPayment.IPayment")]
    public interface IBillPaymentService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IPayment/PayBill", ReplyAction = "http://tempuri.org/IPayment/PayBillResponse")]
        System.Xml.Linq.XElement PayBill(string guid, string tokenKey, System.Xml.Linq.XElement paymentRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBillPaymentServiceChannel : IBillPaymentService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BillPaymentClient : System.ServiceModel.ClientBase<IBillPaymentService>, IBillPaymentService
    {

        public BillPaymentClient()
        {
        }

        public BillPaymentClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public BillPaymentClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillPaymentClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BillPaymentClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement PayBill(string guid, string tokenKey, System.Xml.Linq.XElement paymentRequest)
        {
            return base.Channel.PayBill(guid, tokenKey, paymentRequest);
        }
    }
}
