using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class Change_PaisFabricacion_To_PaisDestino_In_Label : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaisFabricacion",
                table: "Etiqueta",
                newName: "PaisDestino");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaisDestino",
                table: "Etiqueta",
                newName: "PaisFabricacion");
        }
    }
}
