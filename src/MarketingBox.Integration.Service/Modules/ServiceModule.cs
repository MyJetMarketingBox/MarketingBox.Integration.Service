using System.Reflection.Metadata;
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
            builder.RegisterInstance<BridgeServiceWrapper>(new BridgeServiceWrapper(GetBridges()));
            builder.RegisterType<DepositUpdateStorage>().As<IDepositUpdateStorage>().SingleInstance();
            builder.RegisterType<BackgroundService>().SingleInstance().AutoActivate().AsSelf();
            builder.RegisterRegistrationServiceClient(Program.Settings.RegistrationServiceUrl);
        }

        private static (string, IBridgeService)[] GetBridges()
        {
            var bridges = new (string, IBridgeService)[Program.Settings.GetBridges().Count];

            for (int i = 0; i < Program.Settings.GetBridges().Count; i++)
            {
                var bridgeSettings = Program.Settings.GetBridges()[i];
                bridges[i] = (bridgeSettings.Brand, new BridgeServiceClientFactory(bridgeSettings.Url).GetBridgeService());
            }

            return bridges;
        }
    }
}
