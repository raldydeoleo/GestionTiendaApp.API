using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class ProgramacionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Programacion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FechaProduccion = table.Column<DateTime>(type: "Date", nullable: false),
                    FechaRegistro = table.Column<DateTime>(nullable: false),
                    IdProceso = table.Column<string>(nullable: true),
                    IdModulo = table.Column<string>(nullable: true),
                    IdTurno = table.Column<string>(nullable: true),
                    IdProducto = table.Column<string>(nullable: true),
                    UsuarioProgramacion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programacion_Modulo_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulo",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programacion_Proceso_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "Proceso",
                        principalColumn: "IdProceso",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programacion_Turno_IdTurno",
                        column: x => x.IdTurno,
                        principalTable: "Turno",
                        principalColumn: "IdTurno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Programacion_IdModulo",
                table: "Programacion",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_Programacion_IdProceso",
                table: "Programacion",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Programacion_IdTurno",
                table: "Programacion",
                column: "IdTurno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Programacion");
        }
    }
}
