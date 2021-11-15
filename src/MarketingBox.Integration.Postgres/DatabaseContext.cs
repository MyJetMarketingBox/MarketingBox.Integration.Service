using MarketingBox.Integration.Postgres.Entities.Lead;
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

        private const string LeadTableName = "brandintegration";

        public DbSet<LeadEntity> Leads { get; set; }


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

            SetPartnerEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetPartnerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeadEntity>().ToTable(LeadTableName);
            modelBuilder.Entity<LeadEntity>().HasKey(e => e.LeadId);
            modelBuilder.Entity<LeadEntity>().OwnsOne(x => x.BrandInfo);   //modelBuilder.Entity<LeadEntity>().Ignore(x => x.GeneralInfo);
            modelBuilder.Entity<LeadEntity>().OwnsOne(x => x.AdditionalInfo);
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new {e.TenantId, e.LeadId});
        }
            
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
