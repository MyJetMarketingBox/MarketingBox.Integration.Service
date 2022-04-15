using Destructurama.Attributed;
using System;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations
{

    [DataContract]
    public class DepositorReporting
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string CustomerEmail { get; set; }

        [DataMember(Order = 3)]
        public DateTime DepositedAt { get; set; }
    }
}