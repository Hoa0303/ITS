using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class product_coloralt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Details_ProductId",
                table: "Product_Details");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Details_ProductId",
                table: "Product_Details",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Details_ProductId",
                table: "Product_Details");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Details_ProductId",
                table: "Product_Details",
                column: "ProductId");
        }
    }
}
