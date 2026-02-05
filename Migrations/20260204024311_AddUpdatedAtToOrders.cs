using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 4, 2, 43, 11, 246, DateTimeKind.Utc).AddTicks(1156), new DateTime(2026, 2, 4, 2, 43, 11, 246, DateTimeKind.Utc).AddTicks(1156) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 29, 7, 41, 9, 145, DateTimeKind.Utc).AddTicks(7338), new DateTime(2026, 1, 29, 7, 41, 9, 145, DateTimeKind.Utc).AddTicks(7339) });
        }
    }
}
