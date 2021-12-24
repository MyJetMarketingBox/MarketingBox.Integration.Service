using Autofac;


// ReSharper disable UnusedMember.Global

namespace MarketingBox.Integration.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterIntegrationServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new IntegrationServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetIntegrationService()).As<IIntegrationService>().SingleInstance();
        }
    }
}
