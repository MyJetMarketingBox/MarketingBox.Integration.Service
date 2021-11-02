using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.Integration.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxIntegrationService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.RegistrationServiceUrl")]
        public string RegistrationServiceUrl { get; set; }


        [YamlProperty("MarketingBoxIntegrationService.IntegrationMonfexBridgeUrl")]
        public string IntegrationMonfexBridgeUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.IntegrationHandelproBridgeUrl")]
        public string IntegrationHandelproBridgeUrl { get; set; }


        [YamlProperty("MarketingBoxIntegrationService.IntegrationAllianzmarketBridgeUrl")]
        public string IntegrationAllianzmarketBridgeUrl { get; set; }

    }

}
