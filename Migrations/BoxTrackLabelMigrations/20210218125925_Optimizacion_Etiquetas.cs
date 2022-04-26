using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class Optimizacion_Etiquetas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsNula",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "FechaHoraAnulacion",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "UsuarioAnulacion",
                table: "Etiqueta");

            migrationBuilder.RenameColumn(
                name: "Secuencia",
                table: "Etiqueta",
                newName: "SecuenciaInicial");

            migrationBuilder.AddColumn<int>(
                name: "CantidadImpresa",
                table: "Etiqueta",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadImpresa",
                table: "Etiqueta");

            migrationBuilder.RenameColumn(
                name: "SecuenciaInicial",
                table: "Etiqueta",
                newName: "Secuencia");

            migrationBuilder.AddColumn<bool>(
                name: "EsNula",
                table: "Etiqueta",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraAnulacion",
                table: "Etiqueta",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAnulacion",
                table: "Etiqueta",
                nullable: true);
        }
    }
}
