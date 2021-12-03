using System;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations
{

    [DataContract]
    public class DepositorReporting
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string CustomerEmail { get; set; }

        [DataMember(Order = 3)]
        public DateTime DepositedAt { get; set; }
    }
}