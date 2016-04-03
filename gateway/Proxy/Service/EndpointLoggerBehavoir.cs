using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace EfawateerGateway.Proxy.Service
{
    internal sealed class EndpointLoggerBehaviour : IClientMessageInspector, IEndpointBehavior
    {
     
        public EndpointLoggerBehaviour()
        {

        }
     
        #region IClientMessageInspector Members


        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            var log = new StringBuilder("[RESPONSE]").AppendLine();
            log.AppendLine("(Headers)");
            if (reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
            {
                var httpProperties = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
                foreach (var key in httpProperties.Headers.AllKeys)
                    log.AppendFormat("'{0}': '{1}'", key, httpProperties.Headers[key]).AppendLine();
            }

            log.AppendLine("(Body)");
            log.Append(reply);

            Logger.Trace(log.ToString());
        }

        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {

            string requestString = request.ToString();

            int trxIndex = requestString.IndexOf("<BankTrxID>");
            int trxEndIndex = requestString.IndexOf("</BankTrxID>");

            string guid = null;

            if(trxIndex > 0 && trxEndIndex > 0)
            {
                guid = requestString.Substring(trxIndex, trxEndIndex - trxIndex).Replace("<BankTrxID>", string.Empty);
            }

            if (!string.IsNullOrEmpty(guid))
            {
                var header = MessageHeader.CreateHeader("guid", "http://tempuri.org", guid);
                request.Headers.Add(header);
            }
            
            var log = new StringBuilder();
            log.AppendFormat("[REQUEST, Action = '{0}']", request.Headers.Action).AppendLine();
            log.AppendFormat("Endpoint: '{0}'", channel.RemoteAddress.Uri).AppendLine();
            log.AppendLine("(Body)");
            log.Append(request);

            Logger.Trace(log.ToString());
            return request;
        }

        #endregion

        #region IEndpointBehavior

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            //nothing todo
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //nothing todo
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            //nothing todo
        }

        #endregion
      


    }
}
