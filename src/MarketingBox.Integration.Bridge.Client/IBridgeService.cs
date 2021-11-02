using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Integration.Service.Grpc.Models.Reporting;

namespace MarketingBox.Integration.Bridge.Client
{
    [ServiceContract]
    public interface IBridgeService
    {
        [OperationContract]
        Task<RegistrationBridgeResponse> RegisterCustomerAsync(RegistrationBridgeRequest bridgeRequest);
        
        [OperationContract]
        Task<BridgeCountersResponse> GetBridgeCountersPerPeriodAsync(CountersRequest request);
        
        [OperationContract]
        Task<RegistrationsResponse> GetRegistrationsPerPeriodAsync(RegistrationsRequest request);
        
        [OperationContract]
        Task<DepositsResponse> GetDepositsPerPeriodAsync(DepositsRequest request);
    }
}
