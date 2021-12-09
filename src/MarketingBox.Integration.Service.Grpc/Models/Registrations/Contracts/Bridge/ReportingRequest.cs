using MarketingBox.Integration.Service.Domain.Utils;
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

        public static ReportingRequest Create(DateTime currMonth, int pageSize = 100)
        {
            var request = new ReportingRequest()
            {
                DateFrom = CalendarUtils.StartOfMonth(currMonth),
                DateTo = CalendarUtils.EndOfMonth(currMonth),
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
            DateTime nextMonth = DateFrom.AddMonths(1);
            DateFrom = CalendarUtils.StartOfMonth(nextMonth);
            DateTo = CalendarUtils.EndOfMonth(nextMonth);
            PageIndex = 1;
        }
    }
}