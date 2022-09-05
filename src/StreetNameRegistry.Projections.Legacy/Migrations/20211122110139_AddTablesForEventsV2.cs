using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class AddTablesForEventsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "StreetNameId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipalityId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StreetNameDetailsV2",
                schema: "StreetNameRegistryLegacy",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameDetailsV2", x => x.PersistentLocalId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "StreetNameListMunicipality",
                schema: "StreetNameRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryLanguage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameListMunicipality", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "StreetNameListV2",
                schema: "StreetNameRegistryLegacy",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDutchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameFrenchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameGermanSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameEnglishSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HomonymAdditionDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryLanguage = table.Column<int>(type: "int", nullable: true),
                    NisCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameListV2", x => x.PersistentLocalId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "StreetNameNameV2",
                schema: "StreetNameRegistryLegacy",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NameDutch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameDutchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameFrenchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameGermanSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameEnglishSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    IsFlemishRegion = table.Column<bool>(type: "bit", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameNameV2", x => x.PersistentLocalId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameSyndication_PersistentLocalId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                column: "PersistentLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameDetailsV2_MunicipalityId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameDetailsV2_Removed",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameDetailsV2",
                column: "Removed");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_MunicipalityId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_NameDutchSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "NameDutchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_NameEnglishSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "NameEnglishSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_NameFrenchSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "NameFrenchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_NameGermanSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "NameGermanSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_NisCode",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_Removed",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "Removed");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameListV2_Status",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameListV2",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_MunicipalityId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameDutch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameDutchSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameDutchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameEnglish",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameEnglish");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameEnglishSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameEnglishSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameFrench",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameFrench");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameFrenchSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameFrenchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameGerman",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameGerman");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NameGermanSearch",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NameGermanSearch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_NisCode",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_Removed_IsFlemishRegion",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                columns: new[] { "Removed", "IsFlemishRegion" });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_Status",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameNameV2_VersionTimestamp",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameNameV2",
                column: "VersionTimestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetNameDetailsV2",
                schema: "StreetNameRegistryLegacy");

            migrationBuilder.DropTable(
                name: "StreetNameListMunicipality",
                schema: "StreetNameRegistryLegacy");

            migrationBuilder.DropTable(
                name: "StreetNameListV2",
                schema: "StreetNameRegistryLegacy");

            migrationBuilder.DropTable(
                name: "StreetNameNameV2",
                schema: "StreetNameRegistryLegacy");

            migrationBuilder.DropIndex(
                name: "IX_StreetNameSyndication_PersistentLocalId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.AlterColumn<Guid>(
                name: "StreetNameId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
