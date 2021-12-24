using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration
{
    [DataContract]
    public class RegistrationRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 3)]
        public string RegistrationUniqueId { get; set; }

        [DataMember(Order = 4)]
        public RegistrationInfo Info { get; set; }

        [DataMember(Order = 5)]
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 6)]
        public string IntegrationName { get; set; }

        [DataMember(Order = 7)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 8)]
        public long AffiliateId { get; set; }
    }
}
