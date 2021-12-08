﻿using System;
using MarketingBox.Integration.Service.Domain.Registrations;

namespace MarketingBox.Integration.Postgres.Entities
{
    public class RegistrationLogEntity
    {
        public string TenantId { get; set; }
        public long RegistrationId { get; set; }
        public string RegistrationUniqueId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long AffiliateId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CustomerCreatedAt { get; set; }
        public bool Depositor { get; set; }
        public DateTime DepositedAt { get; set; }
        public CrmStatus Crm { get; set; }
        public DateTime CrmUpdatedAt { get; set; }
        public string IntegrationName { get; set; }
        public long IntegrationId { get; set; }
        public long Sequence { get; set; }
    }
}
