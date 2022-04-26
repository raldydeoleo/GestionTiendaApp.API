using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class change_pk_type_process_module_shift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_Proceso_IdProceso",
                table: "Modulo");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Modulo_IdModulo",
                table: "Produccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Proceso_IdProceso",
                table: "Produccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Turno_IdTurno",
                table: "Produccion");

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
                name: "PK_Turno",
                table: "Turno");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proceso",
                table: "Proceso");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo");

            migrationBuilder.DropColumn(
                name: "IdTurno",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "IdProceso",
                table: "Proceso");

            migrationBuilder.DropColumn(
                name: "IdModulo",
                table: "Modulo");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Turno",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Turno",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdTurno",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdProceso",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdModulo",
                table: "Programacion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdTurno",
                table: "Produccion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdProceso",
                table: "Produccion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdModulo",
                table: "Produccion",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Proceso",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Proceso",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdProceso",
                table: "Modulo",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Modulo",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Modulo",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Almacenamiento",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Turno",
                table: "Turno",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proceso",
                table: "Proceso",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_Codigo",
                table: "Turno",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Proceso_Codigo",
                table: "Proceso",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Modulo_Codigo",
                table: "Modulo",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Almacenamiento_Codigo",
                table: "Almacenamiento",
                column: "Codigo");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_Proceso_IdProceso",
                table: "Modulo",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Modulo_IdModulo",
                table: "Produccion",
                column: "IdModulo",
                principalTable: "Modulo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Proceso_IdProceso",
                table: "Produccion",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Turno_IdTurno",
                table: "Produccion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Modulo_IdModulo",
                table: "Programacion",
                column: "IdModulo",
                principalTable: "Modulo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Proceso_IdProceso",
                table: "Programacion",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programacion_Turno_IdTurno",
                table: "Programacion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_Proceso_IdProceso",
                table: "Modulo");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Modulo_IdModulo",
                table: "Produccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Proceso_IdProceso",
                table: "Produccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Produccion_Turno_IdTurno",
                table: "Produccion");

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
                name: "PK_Turno",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_Codigo",
                table: "Turno");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proceso",
                table: "Proceso");

            migrationBuilder.DropIndex(
                name: "IX_Proceso_Codigo",
                table: "Proceso");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo");

            migrationBuilder.DropIndex(
                name: "IX_Modulo_Codigo",
                table: "Modulo");

            migrationBuilder.DropIndex(
                name: "IX_Almacenamiento_Codigo",
                table: "Almacenamiento");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Proceso");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Proceso");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Modulo");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Modulo");

            migrationBuilder.AddColumn<string>(
                name: "IdTurno",
                table: "Turno",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "IdProceso",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "IdModulo",
                table: "Programacion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "IdTurno",
                table: "Produccion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "IdProceso",
                table: "Produccion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "IdModulo",
                table: "Produccion",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "IdProceso",
                table: "Proceso",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdProceso",
                table: "Modulo",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "IdModulo",
                table: "Modulo",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Almacenamiento",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Turno",
                table: "Turno",
                column: "IdTurno");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proceso",
                table: "Proceso",
                column: "IdProceso");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo",
                column: "IdModulo");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_Proceso_IdProceso",
                table: "Modulo",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "IdProceso",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Modulo_IdModulo",
                table: "Produccion",
                column: "IdModulo",
                principalTable: "Modulo",
                principalColumn: "IdModulo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Proceso_IdProceso",
                table: "Produccion",
                column: "IdProceso",
                principalTable: "Proceso",
                principalColumn: "IdProceso",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produccion_Turno_IdTurno",
                table: "Produccion",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "IdTurno",
                onDelete: ReferentialAction.Restrict);

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
