using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class AddSecondaryLanguageToMunicipality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondaryLanguage",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListMunicipality",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryLanguage",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListMunicipality");
        }
    }
}
