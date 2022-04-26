using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class Configuracion_Etiquetas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_TextoEtiqueta_TextoEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.DropTable(
                name: "TextoEtiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Etiqueta_TextoEtiquetaId",
                table: "Etiqueta");

            migrationBuilder.RenameColumn(
                name: "TextoEtiquetaId",
                table: "Etiqueta",
                newName: "ConfiguracionEtiquetaId");

            migrationBuilder.AddColumn<int>(
                name: "LabelConfigId",
                table: "Etiqueta",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfiguracionEtiqueta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdPais = table.Column<string>(nullable: true),
                    ClienteEspecifico = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    TextoCantidad = table.Column<string>(nullable: true),
                    Advertencia = table.Column<string>(nullable: true),
                    TextoPais = table.Column<string>(nullable: true),
                    TipoEtiqueta = table.Column<string>(nullable: true),
                    LlevaLogo = table.Column<bool>(nullable: false),
                    LlevaTextoInferior = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionEtiqueta", x => x.Id);
                });

            migrationBuilder.Sql(@"insert into [BoxTrackLabel].[dbo].[ConfiguracionEtiqueta] Values('DO',null,'La Aurora,  S.A. Tamboril, Santiago, Rep. Dom.',null,null,'Hecho en República Dominicana / Made in Dominican Republic','BOX',0,1);
            insert into ConfiguracionEtiqueta Values('DO',null,null,null,null,null,'BOX',0,0);
            insert into ConfiguracionEtiqueta Values('DO',null,null,null,null,null,'INDIVIDUAL',1,0);
            insert into ConfiguracionEtiqueta Values('DO',null,null,null,null,null,'INDIVIDUAL',0,0);");
            
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_ConfiguracionEtiqueta_LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.DropTable(
                name: "ConfiguracionEtiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Etiqueta_LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.DropColumn(
                name: "LabelConfigId",
                table: "Etiqueta");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionEtiquetaId",
                table: "Etiqueta",
                newName: "TextoEtiquetaId");

            migrationBuilder.CreateTable(
                name: "TextoEtiqueta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Advertencia = table.Column<string>(nullable: true),
                    ClienteEspecifico = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    IdPais = table.Column<string>(nullable: true),
                    TextoCantidad = table.Column<string>(nullable: true),
                    TextoPais = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextoEtiqueta", x => x.Id);
                });

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
