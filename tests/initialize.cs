using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfawateerTests
{
    [TestClass]
    public class initialize
    {
        [TestMethod]
        public void init()
        {
            var gate = new Gateways.EfawateerGateway();

            gate.Initialize(File.ReadAllText("initialize.xml"));
        }

        [TestMethod]
        public void expand_biller_code_from_cyberplat_opertaro_id()
        {
            var gate = new Gateways.EfawateerGateway();
            var actual = gate.ExpandBillerCodeFromCyberplatOpertaroId(700123);
            const int expected = 123;
            Assert.AreEqual(expected, actual);
        }
    }
}
