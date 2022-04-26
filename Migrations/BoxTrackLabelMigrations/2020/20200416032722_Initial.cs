using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proceso",
                columns: table => new
                {
                    IdProceso = table.Column<string>(nullable: false),
                    Descripcion = table.Column<string>(nullable: false),
                    FechaHoraRegistro = table.Column<DateTime>(nullable: false),
                    FechaHoraModificacion = table.Column<DateTime>(nullable: true),
                    FechaHoraBorrado = table.Column<DateTime>(nullable: true),
                    UsuarioRegistro = table.Column<string>(nullable: false),
                    UsuarioModificacion = table.Column<string>(nullable: true),
                    UsuarioEliminacion = table.Column<string>(nullable: true),
                    EstaBorrado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proceso", x => x.IdProceso);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    IdTurno = table.Column<string>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    HoraInicio = table.Column<TimeSpan>(nullable: false),
                    HoraFin = table.Column<TimeSpan>(nullable: false),
                    FechaHoraRegistro = table.Column<DateTime>(nullable: false),
                    FechaHoraModificacion = table.Column<DateTime>(nullable: true),
                    FechaHoraBorrado = table.Column<DateTime>(nullable: true),
                    UsuarioRegistro = table.Column<string>(nullable: false),
                    UsuarioModificacion = table.Column<string>(nullable: true),
                    UsuarioBorrado = table.Column<string>(nullable: true),
                    EstaBorrado = table.Column<bool>(nullable: false),
                    LetraRepresentacion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.IdTurno);
                });

            migrationBuilder.CreateTable(
                name: "Modulo",
                columns: table => new
                {
                    IdModulo = table.Column<string>(nullable: false),
                    Descripcion = table.Column<string>(nullable: false),
                    FechaHoraRegistro = table.Column<DateTime>(nullable: false),
                    FechaHoraModificacion = table.Column<DateTime>(nullable: true),
                    FechaHoraBorrado = table.Column<DateTime>(nullable: true),
                    UsuarioRegistro = table.Column<string>(nullable: false),
                    UsuarioModificacion = table.Column<string>(nullable: true),
                    UsuarioEliminacion = table.Column<string>(nullable: true),
                    IdProceso = table.Column<string>(nullable: true),
                    EstaBorrado = table.Column<bool>(nullable: false),
                    NumeroModulo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulo", x => x.IdModulo);
                    table.ForeignKey(
                        name: "FK_Modulo_Proceso_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "Proceso",
                        principalColumn: "IdProceso",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produccion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdProceso = table.Column<string>(nullable: true),
                    IdTurno = table.Column<string>(nullable: true),
                    IdModulo = table.Column<string>(nullable: true),
                    IdProducto = table.Column<string>(nullable: true),
                    FechaHoraCierreTurno = table.Column<DateTime>(nullable: true),
                    FechaHoraAperturaTurno = table.Column<DateTime>(nullable: false),
                    UsuarioAperturaTurno = table.Column<string>(nullable: false),
                    UsuarioCierreTurno = table.Column<string>(nullable: true),
                    TurnoAbierto = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produccion_Modulo_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulo",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produccion_Proceso_IdProceso",
                        column: x => x.IdProceso,
                        principalTable: "Proceso",
                        principalColumn: "IdProceso",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produccion_Turno_IdTurno",
                        column: x => x.IdTurno,
                        principalTable: "Turno",
                        principalColumn: "IdTurno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProduccionId = table.Column<int>(nullable: false),
                    CodigoBarra = table.Column<string>(nullable: true),
                    CodigoQr = table.Column<string>(nullable: true),
                    DescripcionProducto = table.Column<string>(nullable: true),
                    CantidadCigarros = table.Column<int>(nullable: false),
                    Secuencia = table.Column<int>(nullable: false),
                    Direccion = table.Column<string>(nullable: true),
                    Mensaje = table.Column<string>(nullable: true),
                    PaisFabricacion = table.Column<string>(nullable: true),
                    FechaProduccion = table.Column<DateTime>(type: "Date", nullable: false),
                    FechaHoraCalendario = table.Column<DateTime>(nullable: false),
                    EsNula = table.Column<bool>(nullable: false),
                    FechaHoraAnulacion = table.Column<DateTime>(nullable: false),
                    Almacenamiento = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiqueta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Etiqueta_Produccion_ProduccionId",
                        column: x => x.ProduccionId,
                        principalTable: "Produccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_ProduccionId",
                table: "Etiqueta",
                column: "ProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Modulo_IdProceso",
                table: "Modulo",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Produccion_IdModulo",
                table: "Produccion",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_Produccion_IdProceso",
                table: "Produccion",
                column: "IdProceso");

            migrationBuilder.CreateIndex(
                name: "IX_Produccion_IdTurno",
                table: "Produccion",
                column: "IdTurno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Etiqueta");

            migrationBuilder.DropTable(
                name: "Produccion");

            migrationBuilder.DropTable(
                name: "Modulo");

            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "Proceso");
        }
    }
}
