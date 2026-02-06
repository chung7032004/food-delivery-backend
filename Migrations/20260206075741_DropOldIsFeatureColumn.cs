using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class DropOldIsFeatureColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeature",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 6, 7, 57, 40, 524, DateTimeKind.Utc).AddTicks(3036), new DateTime(2026, 2, 6, 7, 57, 40, 524, DateTimeKind.Utc).AddTicks(3037) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeature",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 4, 2, 43, 11, 246, DateTimeKind.Utc).AddTicks(1156), new DateTime(2026, 2, 4, 2, 43, 11, 246, DateTimeKind.Utc).AddTicks(1156) });
        }
    }
}
