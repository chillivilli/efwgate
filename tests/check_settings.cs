using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gateways
{
    [TestClass]
    public class check_settings
    {
        [TestMethod]
        public void check()
        {
            var gate = new Gateways.EfawateerGateway();

            gate.Initialize(File.ReadAllText("initialize.xml"));

            var result = gate.CheckSettings();
            var b = result == "OK";

            Assert.IsTrue(b);
        }
    }
}
