using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlintoOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "OrderDetials",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "OrderDetials");
        }
    }
}
