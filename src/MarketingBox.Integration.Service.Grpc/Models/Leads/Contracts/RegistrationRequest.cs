using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class RegistrationRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 3)]
        public string LeadUniqueId { get; set; }

        [DataMember(Order = 4)]
        public RegistrationLeadInfo Info { get; set; }

        [DataMember(Order = 5)]
        public RegistrationLeadAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 6)]
        public string IntegrationName { get; set; }

        [DataMember(Order = 7)]
        public long IntegrationId { get; set; }
    }
}
