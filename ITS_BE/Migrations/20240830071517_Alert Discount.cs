using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class AlertDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Disconut",
                table: "Products",
                newName: "Discount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "Products",
                newName: "Disconut");
        }
    }
}
