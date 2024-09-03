using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class Alerttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "Product_Details");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "Disconut");

            migrationBuilder.AlterColumn<string>(
                name: "Battery",
                table: "Product_Details",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "Prices",
                table: "Product_Colors",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Product_Colors",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prices",
                table: "Product_Colors");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Product_Colors");

            migrationBuilder.RenameColumn(
                name: "Disconut",
                table: "Products",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "Battery",
                table: "Product_Details",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<double>(
                name: "price",
                table: "Product_Details",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
