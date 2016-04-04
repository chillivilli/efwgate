using System;
using System.ServiceModel;
using EfawateerGateway.Proxy.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfawateerTests
{
    [TestClass]
    public class authenticate
    {
        [TestMethod]
        public void authenticate_success()
        {
            var token = new TokenServiceClient(new WSHttpBinding(SecurityMode.None, true),
                new EndpointAddress(UriContext.Authenticate));
            var authenticate = token.Authenticate(GenerateGuid(), 158, "Test@1234");

            Assert.IsNotNull(authenticate.Element("MsgHeader"));            
        }

        [TestMethod]
        public void authenticate_fail()
        {
            var token = new TokenServiceClient(new WSHttpBinding(SecurityMode.None, true),
                new EndpointAddress(UriContext.Authenticate));
            var authenticate = token.Authenticate(GenerateGuid(), 158, "Test@1235");

            var o = authenticate.Element("MsgBody");
            Assert.IsNull(o);
        }

        private string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}