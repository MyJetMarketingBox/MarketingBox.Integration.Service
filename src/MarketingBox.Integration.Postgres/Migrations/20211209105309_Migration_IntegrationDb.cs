using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Integration.Postgres.Migrations
{
    public partial class Migration_IntegrationDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration-service");

            migrationBuilder.CreateTable(
                name: "registrationslogs",
                schema: "integration-service",
                columns: table => new
                {
                    RegistrationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    RegistrationUniqueId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    CustomerEmail = table.Column<string>(type: "text", nullable: true),
                    CustomerCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Depositor = table.Column<bool>(type: "boolean", nullable: false),
                    DepositedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Crm = table.Column<int>(type: "integer", nullable: false),
                    CrmUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IntegrationName = table.Column<string>(type: "text", nullable: true),
                    IntegrationId = table.Column<long>(type: "bigint", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrationslogs", x => x.RegistrationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registrationslogs_TenantId_IntegrationId_CustomerId",
                schema: "integration-service",
                table: "registrationslogs",
                columns: new[] { "TenantId", "IntegrationId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_registrationslogs_TenantId_RegistrationId",
                schema: "integration-service",
                table: "registrationslogs",
                columns: new[] { "TenantId", "RegistrationId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registrationslogs",
                schema: "integration-service");
        }
    }
}
