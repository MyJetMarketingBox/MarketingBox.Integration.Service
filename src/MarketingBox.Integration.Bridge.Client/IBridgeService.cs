using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge;

namespace MarketingBox.Integration.Bridge.Client
{
    [ServiceContract]
    public interface IBridgeService
    {
        [OperationContract]
        Task<RegistrationResponse> SendRegistrationAsync(RegistrationRequest request);
        
        [OperationContract]
        Task<ReportingResponse> GetRegistrationsPerPeriodAsync(ReportingRequest request);
    }
}
