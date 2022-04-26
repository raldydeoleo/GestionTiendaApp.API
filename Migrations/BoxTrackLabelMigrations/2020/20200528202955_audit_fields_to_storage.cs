using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class audit_fields_to_storage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaBorrado",
                table: "Almacenamiento",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraBorrado",
                table: "Almacenamiento",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraModificacion",
                table: "Almacenamiento",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraRegistro",
                table: "Almacenamiento",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UsuarioBorrado",
                table: "Almacenamiento",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioModificacion",
                table: "Almacenamiento",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRegistro",
                table: "Almacenamiento",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaBorrado",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "FechaHoraBorrado",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "FechaHoraModificacion",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "FechaHoraRegistro",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "UsuarioBorrado",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacion",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistro",
                table: "Almacenamiento");
        }
    }
}
