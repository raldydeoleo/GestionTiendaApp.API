using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxTrackLabel.API.Migrations.BoxTrackLabelMigrations
{
    public partial class DataMatrix_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataMatrix_Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(nullable: true),
                    ContactPerson = table.Column<string>(nullable: true),
                    CreateMethodType = table.Column<string>(nullable: false),
                    ExpectedStartDate = table.Column<DateTime>(type: "Date", nullable: true),
                    FactoryAddress = table.Column<string>(nullable: true),
                    FactoryCountry = table.Column<string>(nullable: false),
                    FactoryId = table.Column<int>(nullable: false),
                    FactoryName = table.Column<string>(nullable: false),
                    PoNumber = table.Column<int>(nullable: true),
                    ProductCode = table.Column<string>(nullable: false),
                    ProductDescription = table.Column<string>(nullable: false),
                    ProductionLineId = table.Column<int>(nullable: false),
                    ProductionOrderId = table.Column<string>(nullable: true),
                    ReleaseMethodType = table.Column<string>(nullable: false),
                    ServiceProviderId = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true, defaultValue: "Creada"),
                    CisType = table.Column<string>(nullable: false),
                    Gtin = table.Column<string>(nullable: false),
                    Mrp = table.Column<string>(nullable: true, defaultValue: "0000"),
                    Quantity = table.Column<int>(nullable: false),
                    SerialNumberType = table.Column<string>(nullable: true),
                    StickerId = table.Column<int>(nullable: true),
                    TemplateId = table.Column<int>(nullable: false),
                    ExpectedCompleteTimestamp = table.Column<int>(nullable: true),
                    IsPrintAuthorized = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UserCreate = table.Column<string>(nullable: true),
                    PrintAuthorizedDate = table.Column<DateTime>(nullable: true),
                    UserPrintAuthorized = table.Column<string>(nullable: true),
                    IsClose = table.Column<bool>(nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: true),
                    UserClose = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMatrix_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataMatrix_OrderSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OmsId = table.Column<string>(nullable: false),
                    OmsUrl = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: false),
                    ConnectionId = table.Column<string>(nullable: false),
                    ContactPerson = table.Column<string>(nullable: false),
                    CreateMethodType = table.Column<string>(nullable: false),
                    FactoryAddress = table.Column<string>(nullable: false),
                    FactoryCountry = table.Column<string>(nullable: false),
                    FactoryId = table.Column<int>(nullable: false),
                    FactoryName = table.Column<string>(nullable: false),
                    ProductionLineId = table.Column<int>(nullable: false),
                    ReleaseMethodType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMatrix_OrderSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataMatrix_Code",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CisType = table.Column<string>(nullable: true),
                    CodeDescription = table.Column<string>(nullable: false),
                    IsPrinted = table.Column<bool>(nullable: false),
                    IsDropout = table.Column<bool>(nullable: false),
                    DropoutDate = table.Column<DateTime>(nullable: true),
                    UserDropout = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMatrix_Code", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataMatrix_Code_DataMatrix_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "DataMatrix_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataMatrix_Code_OrderId",
                table: "DataMatrix_Code",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataMatrix_Code");

            migrationBuilder.DropTable(
                name: "DataMatrix_OrderSetting");

            migrationBuilder.DropTable(
                name: "DataMatrix_Order");
        }
    }
}
