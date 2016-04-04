using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfawateerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var element = XDocument.Parse(File.ReadAllText(@"d:\private\Ykhanov\BillerList V2 Response.txt"));
        }
    }
}
