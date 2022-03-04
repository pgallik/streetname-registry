using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Wfs.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wfs.streetname");

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: "wfs.streetname",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    DesiredState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesiredStateChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionStates", x => x.Name)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "StreetNameHelper",
                schema: "wfs.streetname",
                columns: table => new
                {
                    StreetNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersistentLocalId = table.Column<int>(type: "int", nullable: true),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    VersionAsString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameHelper", x => x.StreetNameId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "StreetNameHelperV2",
                schema: "wfs.streetname",
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
                    Status = table.Column<int>(type: "int", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    VersionAsString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameHelperV2", x => x.PersistentLocalId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelper_MunicipalityId",
                schema: "wfs.streetname",
                table: "StreetNameHelper",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelper_Removed_Complete",
                schema: "wfs.streetname",
                table: "StreetNameHelper",
                columns: new[] { "Removed", "Complete" });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelperV2_MunicipalityId",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelperV2_Removed",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2",
                column: "Removed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "wfs.streetname");

            migrationBuilder.DropTable(
                name: "StreetNameHelper",
                schema: "wfs.streetname");

            migrationBuilder.DropTable(
                name: "StreetNameHelperV2",
                schema: "wfs.streetname");
        }
    }
}
