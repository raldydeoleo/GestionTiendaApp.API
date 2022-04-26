using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class LabelTextAndLabelRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "Mensaje",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "PaisDestino",
                table: "Etiqueta");

            migrationBuilder.AddColumn<int>(
                name: "TextoEtiquetaId",
                table: "Etiqueta",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_TextoEtiquetaId",
                table: "Etiqueta",
                column: "TextoEtiquetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etiqueta_TextoEtiqueta_TextoEtiquetaId",
                table: "Etiqueta",
                column: "TextoEtiquetaId",
                principalTable: "TextoEtiqueta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_TextoEtiqueta_TextoEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Etiqueta_TextoEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "TextoEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Etiqueta",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mensaje",
                table: "Etiqueta",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaisDestino",
                table: "Etiqueta",
                nullable: true);
        }
    }
}
