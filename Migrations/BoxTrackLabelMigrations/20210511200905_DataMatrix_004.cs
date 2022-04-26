using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class DataMatrix_004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPrinted",
                table: "DataMatrix_Code",
                newName: "IsConfirmed");

            migrationBuilder.AddColumn<string>(
                name: "UserConfirm",
                table: "DataMatrix_Code",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserConfirm",
                table: "DataMatrix_Code");

            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "DataMatrix_Code",
                newName: "IsPrinted");
        }
    }
}
