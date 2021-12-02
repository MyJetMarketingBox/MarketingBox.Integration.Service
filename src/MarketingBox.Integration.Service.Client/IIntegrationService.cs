using System;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;

namespace MarketingBox.Integration.Service.Client
{
    [ServiceContract]
    public interface IIntegrationService
    {
        [OperationContract]
        Task<RegistrationResponse> SendRegisterationAsync(RegistrationRequest request);
    }
}
