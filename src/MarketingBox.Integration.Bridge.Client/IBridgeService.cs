using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Integration.Bridge.Client
{
    [ServiceContract]
    public interface IBridgeService
    {
        [OperationContract]
        Task<Response<CustomerInfo>> SendRegistrationAsync(RegistrationRequest request);
        
        [OperationContract]
        Task<Response<IReadOnlyCollection<RegistrationReporting>>> GetRegistrationsPerPeriodAsync(ReportingRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<DepositorReporting>>> GetDepositorsPerPeriodAsync(ReportingRequest request);
    }
}
