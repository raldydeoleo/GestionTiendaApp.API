using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class scheduleChangeOverCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraFinalizado",
                table: "Programacion",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Programacion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioFinalizado",
                table: "Programacion",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraFinalizado",
                table: "Programacion");

            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Programacion");

            migrationBuilder.DropColumn(
                name: "UsuarioFinalizado",
                table: "Programacion");
        }
    }
}
