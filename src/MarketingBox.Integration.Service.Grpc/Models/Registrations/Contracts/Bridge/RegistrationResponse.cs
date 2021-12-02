using System.Runtime.Serialization;
using MarketingBox.Integration.Service.Grpc.Models.Common;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge
{
    [DataContract]
    public class RegistrationResponse
    {
        [DataMember(Order = 1)]
        public ResultCode ResultCode { get; set; }

        [DataMember(Order = 2)]
        public string ResultMessage { get; set; }

        [DataMember(Order = 3)]
        public CustomerInfo CustomerInfo { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}