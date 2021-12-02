using System;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations
{

    [DataContract]
    public class ReportInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string CustomerEmail { get; set; }

        [DataMember(Order = 3)]
        public bool Depositor  { get; set; }

        [DataMember(Order = 4)]
        public string CrmStatus  { get; set; }

    }
}