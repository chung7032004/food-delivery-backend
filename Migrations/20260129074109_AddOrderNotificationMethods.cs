using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNotificationMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 29, 7, 41, 9, 145, DateTimeKind.Utc).AddTicks(7338), new DateTime(2026, 1, 29, 7, 41, 9, 145, DateTimeKind.Utc).AddTicks(7339) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 29, 7, 30, 43, 216, DateTimeKind.Utc).AddTicks(9972), new DateTime(2026, 1, 29, 7, 30, 43, 216, DateTimeKind.Utc).AddTicks(9972) });
        }
    }
}
