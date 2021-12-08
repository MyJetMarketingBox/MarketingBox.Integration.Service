using System;
using System.Collections.Generic;
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

        [YamlProperty("MarketingBoxIntegrationService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.RegistrationServiceUrl")]
        public string RegistrationServiceUrl { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.Bridges")]
        public Dictionary<string, string> Bridges { get; set; }

        [YamlProperty("MarketingBoxIntegrationService.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        public List<BridgeSettings> GetBridges()
        {
            var bridgesList = new List<BridgeSettings>();
            foreach (var bridgeSetting in Bridges)
            {
                var items = bridgeSetting.Value.Split("@");
                bridgesList.Add(new BridgeSettings()
                {
                    TenantId = items[0],
                    IntegrationName = items[1],
                    IntegrationId = Convert.ToInt64(items[2]),
                    Url = items[3]
                });
            }
            return bridgesList;
        }
    }
}
