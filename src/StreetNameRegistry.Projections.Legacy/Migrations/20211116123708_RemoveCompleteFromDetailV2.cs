using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class RemoveCompleteFromDetailV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complete",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
