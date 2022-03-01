using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class AddLastEventHash_To_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastEventHash",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEventHash",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2");
        }
    }
}
