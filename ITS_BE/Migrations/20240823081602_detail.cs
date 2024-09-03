using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class detail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SizeScreen = table.Column<float>(type: "real", nullable: false),
                    ScanHz = table.Column<string>(type: "text", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: false),
                    RearCam = table.Column<string>(type: "text", nullable: false),
                    FrontCam = table.Column<string>(type: "text", nullable: false),
                    Cpu = table.Column<string>(type: "text", nullable: false),
                    Ram = table.Column<int>(type: "integer", nullable: false),
                    Rom = table.Column<int>(type: "integer", nullable: false),
                    Battery = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Details_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_Details_ProductId",
                table: "Product_Details",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_Details");
        }
    }
}
