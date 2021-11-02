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
            //builder.RegisterSimpleTradingBridgeClient("Monfex",Program.Settings.IntegrationMonfexBridgeUrl);
            //builder.RegisterSimpleTradingBridgeClient("Handelpro", Program.Settings.IntegrationHandelproBridgeUrl);
            //builder.RegisterSimpleTradingBridgeClient("Allianzmarket", Program.Settings.IntegrationAllianzmarketBridgeUrl);
            builder.RegisterInstance<BridgeServiceWrapper>(new BridgeServiceWrapper(
                new (string, IBridgeService)[]
                {
                    ("Monfex", new BridgeServiceClientFactory(Program.Settings.IntegrationMonfexBridgeUrl).GetBridgeService()),
                    ("Handelpro", new BridgeServiceClientFactory(Program.Settings.IntegrationHandelproBridgeUrl).GetBridgeService()),
                    ("Allianzmarket", new BridgeServiceClientFactory(Program.Settings.IntegrationAllianzmarketBridgeUrl).GetBridgeService())
                }));
            builder.RegisterType<DepositUpdateStorage>().As<IDepositUpdateStorage>().SingleInstance();
            builder.RegisterType<BackgroundService>().SingleInstance().AutoActivate().AsSelf();
            builder.RegisterRegistrationServiceClient(Program.Settings.RegistrationServiceUrl);
        }
    }
}
