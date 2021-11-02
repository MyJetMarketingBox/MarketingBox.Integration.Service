using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;

namespace MarketingBox.Integration.Service.Client
{
    [ServiceContract]
    public interface IIntegrationService
    {
        [OperationContract]
        Task<RegistrationResponse> RegisterLeadAsync(RegistrationRequest request);
    }
}
