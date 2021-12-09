using Destructurama.Attributed;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations
{
    [DataContract]
    public class
        CustomerInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string LoginUrl { get; set; }
    }
}