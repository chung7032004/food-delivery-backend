using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatedOrderIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RelatedOrderId",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 2, 1, 12, 54, 896, DateTimeKind.Utc).AddTicks(9654), new DateTime(2026, 2, 2, 1, 12, 54, 896, DateTimeKind.Utc).AddTicks(9655) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedOrderId",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 2, 0, 35, 8, 66, DateTimeKind.Utc).AddTicks(5454), new DateTime(2026, 2, 2, 0, 35, 8, 66, DateTimeKind.Utc).AddTicks(5454) });
        }
    }
}
