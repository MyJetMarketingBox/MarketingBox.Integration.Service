using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Integration.Postgres.Migrations
{
    public partial class Remove_Sequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "integration-service",
                table: "registrationslogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "integration-service",
                table: "registrationslogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
