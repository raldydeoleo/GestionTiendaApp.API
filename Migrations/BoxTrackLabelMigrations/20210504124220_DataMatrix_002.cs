using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class DataMatrix_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodeDescription",
                table: "DataMatrix_Code",
                newName: "CodeValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodeValue",
                table: "DataMatrix_Code",
                newName: "CodeDescription");
        }
    }
}
