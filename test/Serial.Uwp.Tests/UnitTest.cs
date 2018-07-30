
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dandy.Devices.Serial.Uwp.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestFactoryGetAllAsync()
        {
            var factory = new Factory();
            var x = factory.FindAllAsync().Result;
            Assert.IsNotNull(x);
        }
    }
}
