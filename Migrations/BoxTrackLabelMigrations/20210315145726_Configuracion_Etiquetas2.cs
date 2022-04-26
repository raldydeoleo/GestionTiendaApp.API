using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class Configuracion_Etiquetas2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_ConfiguracionEtiqueta_LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Etiqueta_LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_ConfiguracionEtiquetaId",
                table: "Etiqueta",
                column: "ConfiguracionEtiquetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etiqueta_ConfiguracionEtiqueta_ConfiguracionEtiquetaId",
                table: "Etiqueta",
                column: "ConfiguracionEtiquetaId",
                principalTable: "ConfiguracionEtiqueta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_ConfiguracionEtiqueta_ConfiguracionEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Etiqueta_ConfiguracionEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.AddColumn<int>(
                name: "LabelConfigId",
                table: "Etiqueta",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_LabelConfigId",
                table: "Etiqueta",
                column: "LabelConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etiqueta_ConfiguracionEtiqueta_LabelConfigId",
                table: "Etiqueta",
                column: "LabelConfigId",
                principalTable: "ConfiguracionEtiqueta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
