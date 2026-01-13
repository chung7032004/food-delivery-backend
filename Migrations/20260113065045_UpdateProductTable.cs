using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    public partial class UpdateProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<bool>(
        name: "IsFeatured",
        table: "Products",
        type: "boolean",
        nullable: false,
        defaultValue: false);

    migrationBuilder.AddColumn<DateTime?>(
        name: "UpdatedAt",
        table: "Products",
        type: "timestamp with time zone",
        nullable: true);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "IsFeatured",
        table: "Products");

    migrationBuilder.DropColumn(
        name: "UpdatedAt",
        table: "Products");
}


    }
}
