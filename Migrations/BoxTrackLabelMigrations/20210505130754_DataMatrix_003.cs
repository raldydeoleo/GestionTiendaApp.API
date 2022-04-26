using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class DataMatrix_003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "DataMatrix_Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "DataMatrix_Order");
        }
    }
}
