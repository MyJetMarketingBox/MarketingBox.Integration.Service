using System;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Integration.Service.Client
{
    [ServiceContract]
    public interface IIntegrationService
    {
        [OperationContract]
        Task<Response<Registration>> SendRegisterationAsync(RegistrationRequest request);
    }
}
