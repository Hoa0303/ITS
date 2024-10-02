﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITS_BE.Migrations
{
    /// <inheritdoc />
    public partial class img_color : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Product_Colors",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Product_Colors");
        }
    }
}