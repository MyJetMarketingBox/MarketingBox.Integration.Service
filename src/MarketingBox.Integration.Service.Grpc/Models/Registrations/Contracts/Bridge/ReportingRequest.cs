using System;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge
{
    public class ReportingRequest
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public DateTime DateFrom { get; set; }

        [DataMember(Order = 3)]
        public DateTime DateTo { get; set; }

        [DataMember(Order = 4)]
        public int PageIndex { get; set; }


        [DataMember(Order = 5)]
        public int PageCount { get; set; }

    }
}