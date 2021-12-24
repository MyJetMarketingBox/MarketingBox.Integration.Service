using MarketingBox.Integration.Service.Domain.Registrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketingBox.Integration.Service.Domain.Repositories
{
    public interface IRegistrationsLogRepository
    {
        Task SaveAsync(RegistrationLog registration);
        Task<RegistrationLog> GetByCustomerIdAsync(string tenantId, string customerId);
        Task<IReadOnlyCollection<RegistrationLog>> GetPotentionalDepositorsByBrandAsync(string tenantId, long brandId);
        Task<IReadOnlyCollection<RegistrationLog>> GetPotentionalDepositorsByCustomersAsync(string tenantId, IReadOnlyCollection<string> customers);
    }
}