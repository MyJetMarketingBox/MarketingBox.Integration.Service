using System;
using MarketingBox.Integration.Service.Utils;
using NUnit.Framework;

namespace MarketingBox.Integration.Service.Tests
{
    public class LanguageTest
    {
        [TestCase("99.99.99.22", "EN")]
        [TestCase("ES", "CA")]
        [TestCase("UA", "UK")]
        [TestCase("PL", "PL")]
        public void Test(string country, string language)
        {
            var search = LanguageUtils.GetIso2LanguageFromCountryFirstOrDefault(country);
            Assert.AreEqual(language, search);
        }
    }
}
