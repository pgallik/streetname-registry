using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Extract.Migrations
{
    public partial class RemoveHelperTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetName_Municipalities",
                schema: "StreetNameRegistryExtract");

            migrationBuilder.DropColumn(
                name: "HomonymUnknown",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2");

            migrationBuilder.DropColumn(
                name: "NameUnknown",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomonymUnknown",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameUnknown",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StreetName_Municipalities",
                schema: "StreetNameRegistryExtract",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetName_Municipalities", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }
    }
}
