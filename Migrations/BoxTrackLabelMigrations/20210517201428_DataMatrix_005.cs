using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class DataMatrix_005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataMatrixOrderId",
                table: "Produccion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmDate",
                table: "DataMatrix_Code",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "DataMatrix_Code",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LabelId",
                table: "DataMatrix_Code",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrintDate",
                table: "DataMatrix_Code",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPrint",
                table: "DataMatrix_Code",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataMatrixOrderId",
                table: "Produccion");

            migrationBuilder.DropColumn(
                name: "ConfirmDate",
                table: "DataMatrix_Code");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "DataMatrix_Code");

            migrationBuilder.DropColumn(
                name: "LabelId",
                table: "DataMatrix_Code");

            migrationBuilder.DropColumn(
                name: "PrintDate",
                table: "DataMatrix_Code");

            migrationBuilder.DropColumn(
                name: "UserPrint",
                table: "DataMatrix_Code");
        }
    }
}
