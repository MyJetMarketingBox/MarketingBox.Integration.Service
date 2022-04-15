using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Integration.Postgres.Extensions;
using MarketingBox.Integration.Service.Domain.Registrations;
using MarketingBox.Integration.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Linq;
using MarketingBox.Integration.Postgres.Entities;
using MarketingBox.Sdk.Common.Exceptions;

namespace MarketingBox.Integration.Postgres.Repositories
{
    public class RegistrationsLogRepository : IRegistrationsLogRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public RegistrationsLogRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<RegistrationLog> GetByCustomerIdAsync(
            string tenantId, string customerId)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var registrationLog = await ctx.RegistrationsLog
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.CustomerId == customerId);

            if (registrationLog == null)
            {
                throw new NotFoundException($"Registration with {nameof(customerId)}", customerId);
            }

            return registrationLog.CreateRegistrationLog();
        }

        public async Task<IReadOnlyCollection<RegistrationLog>> GetPotentionalDepositorsByBrandAsync(string tenantId, long integrationId)
        {
            return await GetRegistrationsByBrandAsync(tenantId, integrationId, false);
        }

        private async Task<IReadOnlyCollection<RegistrationLog>> GetRegistrationsByBrandAsync(string tenantId, long integrationId, bool depositor)
        {
            var searchQuery = $@"
            SELECT 
            r.""TenantId"",
            r.""RegistrationId"", 
            r.""RegistrationUniqueId"",
            r.""CreatedAt"", 
            r.""AffiliateId"", 
            r.""CustomerId"",
            r.""CustomerEmail"",
            r.""CustomerCreatedAt"",
            r.""Depositor"", 
            r.""DepositedAt"",
            r.""Crm"",
            r.""CrmUpdatedAt"", 
            r.""IntegrationName"",
            r.""IntegrationId"",
            r.""Sequence""
            FROM ""integration-service"".registrationslogs AS r
            WHERE r.""TenantId"" = @TenantId AND r.""IntegrationId"" = @IntegrationId AND r.""Depositor"" = {depositor}
            ORDER BY r.""RegistrationId"" ASC";

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var array = await context.Database.GetDbConnection()
                    .QueryAsync<RegistrationLogEntity>(searchQuery,
                    new
                    {
                        TenantId = tenantId,
                        IntegrationId = integrationId
                    });

                var registrationLogEntities = array.ToList();
                
                if (!registrationLogEntities.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }
                
                var response = registrationLogEntities
                    .Select(registration => registration.CreateRegistrationLog())
                    .ToList();

                return response;
        }

        public async Task<IReadOnlyCollection<RegistrationLog>> GetPotentionalDepositorsByCustomersAsync(string tenantId, IReadOnlyCollection<string> customerIds)
        {
            var where = $@" and ""CustomerId"" IN ({String.Join(", ", customerIds)})";

            var searchQuery = $@"
            SELECT 
            r.""TenantId"",
            r.""RegistrationId"", 
            r.""RegistrationUniqueId"",
            r.""CreatedAt"", 
            r.""AffiliateId"", 
            r.""CustomerId"",
            r.""CustomerEmail"",
            r.""CustomerCreatedAt"",
            r.""Depositor"", 
            r.""DepositedAt"",
            r.""Crm"",
            r.""CrmUpdatedAt"", 
            r.""IntegrationName"",
            r.""IntegrationId"",
            r.""Sequence""
            FROM ""integration-service"".registrationslogs AS r
            WHERE r.""TenantId"" = @TenantId AND r.""Depositor"" = false
            {where}
            ORDER BY r.""RegistrationId"" ASC";

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var array = await context.Database.GetDbConnection()
                    .QueryAsync<RegistrationLogEntity>(searchQuery, 
                    new
                    {
                        TenantId = tenantId,
                    });

                var registrationLogEntities = array.ToList();
                
                if (!registrationLogEntities.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }
                var response = registrationLogEntities
                    .Select(registration => registration.CreateRegistrationLog())
                    .ToList();

                return response;
            }

        public async Task SaveAsync(RegistrationLog registration)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = registration.CreateRegistrationLogEntity();
            var rowsCount = await ctx.RegistrationsLog.Upsert(entity)
                .AllowIdentityMatch()
                .UpdateIf(prev => prev.Sequence < entity.Sequence)
                .RunAsync();

            if (rowsCount == 0)
            {
                throw new AlreadyExistsException($"Registration {registration.RegistrationId} already updated, try to use most recent version");
            }
        }
    }
}