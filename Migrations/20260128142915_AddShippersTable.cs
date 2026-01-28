using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class AddShippersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Shipper_ShipperId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipper_Users_UserId",
                table: "Shipper");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipper",
                table: "Shipper");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.RenameTable(
                name: "Shipper",
                newName: "Shippers");

            migrationBuilder.RenameIndex(
                name: "IX_Shipper_UserId",
                table: "Shippers",
                newName: "IX_Shippers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 28, 14, 29, 14, 526, DateTimeKind.Utc).AddTicks(554), new DateTime(2026, 1, 28, 14, 29, 14, 526, DateTimeKind.Utc).AddTicks(555) });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Shippers_ShipperId",
                table: "OrderDetails",
                column: "ShipperId",
                principalTable: "Shippers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shippers_Users_UserId",
                table: "Shippers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Shippers_ShipperId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippers_Users_UserId",
                table: "Shippers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers");

            migrationBuilder.RenameTable(
                name: "Shippers",
                newName: "Shipper");

            migrationBuilder.RenameIndex(
                name: "IX_Shippers_UserId",
                table: "Shipper",
                newName: "IX_Shipper_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipper",
                table: "Shipper",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "RestaurantProfiles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 22, 9, 20, 57, 514, DateTimeKind.Utc).AddTicks(5655), new DateTime(2026, 1, 22, 9, 20, 57, 514, DateTimeKind.Utc).AddTicks(5656) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Customer" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Shipper" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Admin" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Shipper_ShipperId",
                table: "OrderDetails",
                column: "ShipperId",
                principalTable: "Shipper",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipper_Users_UserId",
                table: "Shipper",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
