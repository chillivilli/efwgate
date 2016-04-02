namespace EfawateerGateway.Proxy.Service
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "BillPayment.IPayment")]
    public interface IPayment
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IPayment/PayBill", ReplyAction = "http://tempuri.org/IPayment/PayBillResponse")]
        System.Xml.Linq.XElement PayBill(string guid, string tokenKey, System.Xml.Linq.XElement paymentRequest);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPaymentChannel : IPayment, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PaymentClient : System.ServiceModel.ClientBase<IPayment>, IPayment
    {

        public PaymentClient()
        {
        }

        public PaymentClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public PaymentClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PaymentClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public PaymentClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Xml.Linq.XElement PayBill(string guid, string tokenKey, System.Xml.Linq.XElement paymentRequest)
        {
            return base.Channel.PayBill(guid, tokenKey, paymentRequest);
        }
    }
}
