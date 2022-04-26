using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class CambioPKProgramacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Modulo_IdModulo",
                table: "Programacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Proceso_IdProceso",
                table: "Programacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Programacion");

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdProceso",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdModulo",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion",
                columns: new[] { "FechaProduccion", "IdProceso", "IdModulo", "IdTurno" });

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Modulo_IdModulo",
                table: "Programacion",
                column: "IdModulo",
                principalTable: "Modulo",
                principalColumn: "IdModulo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Proceso_IdProceso",
                table: "Programacion",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "IdProceso",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "IdTurno",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Modulo_IdModulo",
                table: "Programacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Proceso_IdProceso",
                table: "Programacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion");

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IdModulo",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IdProceso",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Programacion",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programacion",
                table: "Programacion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Modulo_IdModulo",
                table: "Programacion",
                column: "IdModulo",
                principalTable: "Modulo",
                principalColumn: "IdModulo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Proceso_IdProceso",
                table: "Programacion",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "IdProceso",
                onDelete: ReferentialAction.Restrict);

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
