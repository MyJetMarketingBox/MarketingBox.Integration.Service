using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge
{
    [DataContract]
    public class RegistrationRequest
    {
        [DataMember(Order = 1)]
        public RegistrationInfo Info { get; set; }

        [DataMember(Order = 2)]
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }
    }
}
