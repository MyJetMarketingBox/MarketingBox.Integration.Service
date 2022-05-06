using Autofac;
using MarketingBox.Integration.Bridge.Client;
using MarketingBox.Integration.Service.Services;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Registration.Service.Client;

namespace MarketingBox.Integration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new BridgeStorage(GetBridges()));
            builder.RegisterRegistrationServiceClient(Program.Settings.RegistrationServiceUrl);
            builder.RegisterType<DepositorsBackgroundService>().SingleInstance().AutoActivate().AsSelf();
            builder.RegisterType<RegistrationsBackgroundService>().SingleInstance().AutoActivate().AsSelf();
        }

        private static (long, MarketingBox.Integration.Service.Storage.Bridge)[] GetBridges()
        {
            var bridges =
                new (long, MarketingBox.Integration.Service.Storage.Bridge)[Program.Settings.GetBridges().Count];

            for (int i = 0; i < Program.Settings.GetBridges().Count; i++)
            {
                var bridgeSettings = Program.Settings.GetBridges()[i];
                bridges[i] = (bridgeSettings.IntegrationId, new MarketingBox.Integration.Service.Storage.Bridge
                {
                    IntegrationId = bridgeSettings.IntegrationId,
                    IntegrationName = bridgeSettings.IntegrationName,
                    TenantId = bridgeSettings.TenantId,
                    Service = new BridgeServiceClientFactory(bridgeSettings.Url).GetBridgeService()
                });
            }

            return bridges;
        }
    }
}