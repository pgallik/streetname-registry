using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Infrastructure.Migrations
{
    public partial class AddSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
              CREATE SEQUENCE {Schema.Sequence}.{SequenceContext.StreetNamePersistentLocalIdSequenceName}
                AS int
                START WITH 3000000
                INCREMENT BY 1
	            MINVALUE 3000000
                MAXVALUE 999999999
                NO CYCLE
                NO CACHE
            ;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP SEQUENCE {Schema.Sequence}.{SequenceContext.StreetNamePersistentLocalIdSequenceName};");
        }
    }
}
