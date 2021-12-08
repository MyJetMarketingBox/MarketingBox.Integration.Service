using MyYamlParser;

namespace MarketingBox.Integration.Service.Settings
{
    public class BridgeSettings
    {
        [YamlProperty("Tenant", null)]
        public string TenantId;
        [YamlProperty("Brand", null)]
        public string IntegrationName;
        [YamlProperty("BrandId", null)]
        public long IntegrationId;
        [YamlProperty("Url", null)]
        public string Url;
    }
}