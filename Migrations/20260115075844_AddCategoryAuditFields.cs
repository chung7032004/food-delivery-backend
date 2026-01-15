using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterColumn<string>(
        name: "Description",
        table: "Categories",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "text");

    migrationBuilder.AddColumn<DateTime>(
        name: "CreatedAt",
        table: "Categories",
        type: "timestamptz",
        nullable: false,
        defaultValueSql: "now()");

    migrationBuilder.AddColumn<DateTime>(
        name: "UpdatedAt",
        table: "Categories",
        type: "timestamptz",
        nullable: true);
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "CreatedAt",
        table: "Categories");

    migrationBuilder.DropColumn(
        name: "UpdatedAt",
        table: "Categories");

    migrationBuilder.AlterColumn<string>(
        name: "Description",
        table: "Categories",
        type: "text",
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "text",
        oldNullable: true);
}

    }
}
