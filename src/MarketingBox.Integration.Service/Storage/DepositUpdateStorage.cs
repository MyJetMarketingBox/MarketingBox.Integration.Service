using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Destructurama.Attributed;


namespace MarketingBox.Integration.Service.Storage
{
    public interface IDepositUpdateStorage
    {
        void Add(string requestLeadUniqueId, DepositUpdateMessage depositUpdateMessage);
        List<KeyValuePair<string, DepositUpdateMessage>> GetAll();
        void Remove(string requestLeadUniqueId);
    }

    public class DepositUpdateMessage
    {
        public string TenantId { get; set; }
        public long RegistrationId { get; set; }
        public string IntegrationName { get; set; }
        public long Sequence { get; set; }
        public long IntegrationId { get; set; }
        public long AffiliateId { get; set; }
        public long BrandId { get; set; }
        public long CampaignId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RegisterDate { get; set; }
        public string CustomerId { get; set; }
        public string UniqueId { get; set; }
        public string Country { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }
        public LeadApprovedType Approved { get; set; }
        public DateTime? ConversionDate { get; set; }
        public string BrandStatus { get; set; }
        public long DepositId { get; set; }
    }

    public enum LeadApprovedType
    {
        Unknown = 0,
        Declined = 1,
        Approved = 2,
        ApprovedManually = 3,
        ApprovedFromCrm = 4
    }

    public class DepositUpdateStorage : IDepositUpdateStorage
    {
        private ConcurrentDictionary<string, DepositUpdateMessage> _data = new ConcurrentDictionary<string, DepositUpdateMessage>();
        public void Add(string requestLeadUniqueId, DepositUpdateMessage depositUpdateMessage)
        {
            _data[requestLeadUniqueId] = depositUpdateMessage;
        }

        public List<KeyValuePair<string, DepositUpdateMessage>> GetAll()
        {
            return _data.ToList();
        }

        public void Remove(string requestLeadUniqueId)
        {
            _data.TryRemove(requestLeadUniqueId, out _);
        }
    }
}