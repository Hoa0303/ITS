using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class UserAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryAddresses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Province_code = table.Column<int>(type: "integer", nullable: true),
                    Province_name = table.Column<string>(type: "text", nullable: true),
                    District_code = table.Column<int>(type: "integer", nullable: true),
                    District_name = table.Column<string>(type: "text", nullable: true),
                    Ward_code = table.Column<int>(type: "integer", nullable: true),
                    Ward_name = table.Column<string>(type: "text", nullable: true),
                    Detail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddresses", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_DeliveryAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "AspNetUsers");
        }
    }
}
