using MyYamlParser;

namespace MarketingBox.Integration.Service.Settings
{
    public class BridgeSettings
    {
        [YamlProperty("Tenant", null)]
        public string Tenant;
        [YamlProperty("Brand", null)]
        public string Brand;
        [YamlProperty("Url", null)]
        public string Url;
    }
}