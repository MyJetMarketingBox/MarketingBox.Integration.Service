using MarketingBox.Integration.Service.Grpc.Models.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge
{
    public class RegistrationsReportingResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<RegistrationReporting> Items { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}