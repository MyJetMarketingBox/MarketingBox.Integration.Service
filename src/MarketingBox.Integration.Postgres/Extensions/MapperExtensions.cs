using MarketingBox.Integration.Postgres.Entities;

namespace MarketingBox.Integration.Postgres.Extensions
{
    public static class MapperExtensions
    {
        public static RegistrationLogEntity CreateRegistrationLogEntity(
            this Service.Domain.Registrations.RegistrationLog registration)
        {
            return new RegistrationLogEntity()
            {
                CustomerId = registration.CustomerId,   
                IntegrationId = registration.IntegrationId, 
                RegistrationId = registration.RegistrationId,   
                CustomerEmail = registration.CustomerEmail,
                AffiliateId = registration.AffiliateId,
                CreatedAt = registration.CreatedAt,
                Crm = registration.Crm,
                CrmUpdatedAt = registration.CrmUpdatedAt,
                CustomerCreatedAt = registration.CustomerCreatedAt,
                DepositedAt = registration.DepositedAt,
                Depositor = registration.Depositor,
                IntegrationName = registration.IntegrationName,
                RegistrationUniqueId = registration.RegistrationUniqueId,
                TenantId = registration.TenantId
            };
        }

        public static Service.Domain.Registrations.RegistrationLog CreateRegistrationLog(
            this RegistrationLogEntity registration)
        {
            return new Service.Domain.Registrations.RegistrationLog()
            {
                CustomerId = registration.CustomerId,
                IntegrationId = registration.IntegrationId,
                RegistrationId = registration.RegistrationId,
                CustomerEmail = registration.CustomerEmail,
                AffiliateId = registration.AffiliateId,
                CreatedAt = registration.CreatedAt,
                Crm = registration.Crm,
                CrmUpdatedAt = registration.CrmUpdatedAt,
                CustomerCreatedAt = registration.CustomerCreatedAt,
                DepositedAt = registration.DepositedAt,
                Depositor = registration.Depositor,
                IntegrationName = registration.IntegrationName,
                RegistrationUniqueId = registration.RegistrationUniqueId,
                TenantId = registration.TenantId
            };
        }
    }
}
