using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddDataForRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RestaurantProfiles",
                columns: new[] { "Id", "Address", "CloseTime", "ClosingMessage", "CreatedAt", "IsOpen", "Latitude", "Longitude", "Name", "OpenTime", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "319 Hùng Vương, P. Vĩnh Trung, Q. Thanh Khê, Đà Nẵng", new TimeSpan(0, 22, 0, 0, 0), null, new DateTime(2026, 1, 22, 9, 20, 57, 514, DateTimeKind.Utc).AddTicks(5655), true, 16.067771, 108.214287, "Food Delivery Shop", new TimeSpan(0, 8, 0, 0, 0), "0909123456", new DateTime(2026, 1, 22, 9, 20, 57, 514, DateTimeKind.Utc).AddTicks(5656) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
