using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetNameRegistry.Projections.Wfs.Migrations
{
    public partial class AddIndexCompleteRemovedIncludes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StreetNameHelperV2_Removed",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2");

            migrationBuilder.DropIndex(
                name: "IX_StreetNameHelper_Removed_Complete",
                schema: "wfs.streetname",
                table: "StreetNameHelper");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelperV2_Removed",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2",
                column: "Removed")
                .Annotation("SqlServer:Include", new[] { "NisCode", "PersistentLocalId" });

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelper_Removed_Complete",
                schema: "wfs.streetname",
                table: "StreetNameHelper",
                columns: new[] { "Removed", "Complete" })
                .Annotation("SqlServer:Include", new[] { "NisCode", "StreetNameId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StreetNameHelperV2_Removed",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2");

            migrationBuilder.DropIndex(
                name: "IX_StreetNameHelper_Removed_Complete",
                schema: "wfs.streetname",
                table: "StreetNameHelper");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelperV2_Removed",
                schema: "wfs.streetname",
                table: "StreetNameHelperV2",
                column: "Removed");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameHelper_Removed_Complete",
                schema: "wfs.streetname",
                table: "StreetNameHelper",
                columns: new[] { "Removed", "Complete" });
        }
    }
}
