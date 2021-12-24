using Autofac;
using MarketingBox.Integration.Postgres.Repositories;
using MarketingBox.Integration.Service.Domain.Repositories;

namespace MarketingBox.Integration.Service.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationsLogRepository>().As<IRegistrationsLogRepository>().InstancePerDependency();
        }
    }
}
