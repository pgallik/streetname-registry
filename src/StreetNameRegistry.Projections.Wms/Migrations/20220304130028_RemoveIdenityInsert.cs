using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Wms.Migrations
{
    public partial class RemoveIdenityInsert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PersistentLocalId",
                schema: "wms.streetname",
                table: "StreetNameHelperV2",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PersistentLocalId",
                schema: "wms.streetname",
                table: "StreetNameHelperV2",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
