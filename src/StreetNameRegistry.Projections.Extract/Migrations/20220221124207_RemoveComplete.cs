using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Extract.Migrations
{
    public partial class RemoveComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complete",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
