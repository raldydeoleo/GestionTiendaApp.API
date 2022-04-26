using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class table_configuracion_valor_col_numeroModulo_to_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NumeroModulo",
                table: "Modulo",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "ConfiguracionValor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(nullable: true),
                    TextoConfiguracion = table.Column<string>(nullable: true),
                    ValorConfiguracion = table.Column<string>(nullable: true),
                    EstaBorrado = table.Column<bool>(nullable: false),
                    FechaHoraBorrado = table.Column<DateTime>(nullable: true),
                    FechaHoraModificacion = table.Column<DateTime>(nullable: true),
                    FechaHoraRegistro = table.Column<DateTime>(nullable: false),
                    UsuarioRegistro = table.Column<string>(nullable: true),
                    UsuarioModificacion = table.Column<string>(nullable: true),
                    UsuarioEliminacion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionValor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionValor_Codigo",
                table: "ConfiguracionValor",
                column: "Codigo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionValor");

            migrationBuilder.AlterColumn<int>(
                name: "NumeroModulo",
                table: "Modulo",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
