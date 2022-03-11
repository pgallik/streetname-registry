using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Api.BackOffice.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "StreetNameRegistryBackOffice");

            migrationBuilder.CreateTable(
                name: "MunicipalityIdByPersistentLocalId",
                schema: "StreetNameRegistryBackOffice",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityIdByPersistentLocalId", x => x.PersistentLocalId)
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityIdByPersistentLocalId",
                schema: "StreetNameRegistryBackOffice");
        }
    }
}
