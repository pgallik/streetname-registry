using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Extract.Migrations
{
    public partial class AddExtractV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "StreetNameV2",
                schema: "StreetNameRegistryExtract",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreetNamePersistentLocalId = table.Column<int>(type: "int", nullable: false),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameUnknown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymUnknown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DbaseRecord = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameV2", x => new { x.MunicipalityId, x.StreetNamePersistentLocalId })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameV2_StreetNamePersistentLocalId",
                schema: "StreetNameRegistryExtract",
                table: "StreetNameV2",
                column: "StreetNamePersistentLocalId")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetName_Municipalities",
                schema: "StreetNameRegistryExtract");

            migrationBuilder.DropTable(
                name: "StreetNameV2",
                schema: "StreetNameRegistryExtract");
        }
    }
}
