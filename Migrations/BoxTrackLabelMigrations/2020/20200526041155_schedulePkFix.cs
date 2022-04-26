using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class schedulePkFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion");

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdProducto",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion",
                columns: new[] { "FechaProduccion", "IdProceso", "IdModulo", "IdTurno", "IdProducto" });

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "IdTurno",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion");

            migrationBuilder.AlterColumn<string>(
                name: "IdProducto",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion",
                columns: new[] { "FechaProduccion", "IdProceso", "IdModulo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "IdTurno",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
