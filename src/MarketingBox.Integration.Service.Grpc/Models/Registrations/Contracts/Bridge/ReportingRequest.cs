using System;
using System.Runtime.Serialization;

namespace MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge
{
    [DataContract]
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
        public int PageSize { get; set; }
        
        public static ReportingRequest Create(DateTime startFrom, int pageSize = 100)
        {
            var request = new ReportingRequest()
            {
                DateFrom = startFrom,
                DateTo = startFrom.AddMonths(1),
                PageIndex = 1,
                PageSize = pageSize,
            };
            return request;
        }

        public void NextPage()
        {
            PageIndex++;
        }

        public void NextMonth()
        {
            DateFrom = DateFrom.AddMonths(1);
            DateTo = DateTo.AddMonths(1);
            PageIndex = 1;
        }
    }
}