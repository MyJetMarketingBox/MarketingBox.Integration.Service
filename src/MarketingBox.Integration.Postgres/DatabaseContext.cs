using MarketingBox.Integration.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;

namespace MarketingBox.Integration.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "integration-service";

        private const string RegistrationsLogTableName = "registrationslogs";

        public DbSet<RegistrationLogEntity> RegistrationsLog { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetRegistrationLogEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetRegistrationLogEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistrationLogEntity>().ToTable(RegistrationsLogTableName);
            modelBuilder.Entity<RegistrationLogEntity>().HasKey(e => e.RegistrationId);
            modelBuilder.Entity<RegistrationLogEntity>().HasIndex(e => new {e.TenantId, e.RegistrationId});
            modelBuilder.Entity<RegistrationLogEntity>().HasIndex(e => new {e.TenantId, e.IntegrationId, e.CustomerId });
        }
            
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
