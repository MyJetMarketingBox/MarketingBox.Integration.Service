using System;
using MarketingBox.Integration.Service.Utils;
using NUnit.Framework;

namespace MarketingBox.Integration.Service.Tests
{
    public class TestExample
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Debug output");
            Assert.Pass();
        }
    }
}
