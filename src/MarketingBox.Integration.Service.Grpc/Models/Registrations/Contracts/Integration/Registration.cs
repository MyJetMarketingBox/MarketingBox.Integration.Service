using System.Runtime.Serialization;
using MarketingBox.Integration.Service.Grpc.Models.Common;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration
{
    [DataContract]
    public class Registration
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public CustomerInfo Customer { get; set; }

        [DataMember(Order = 3)]
        public string FallbackUrl { get; set; }

        [DataMember(Order = 4)]
        public RegistrationInfo OriginalData { get; set; }
    }
}