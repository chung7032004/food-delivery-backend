using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddressAndAddDefaultConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Addresses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Addresses",
                type: "timestamp with time zone",
                nullable: true);

            // -- Enforce business rule: mỗi User chỉ được có 1 địa chỉ mặc định (IsDefault = true)
            // -- Đây là PARTIAL UNIQUE INDEX của PostgreSQL
            // -- Chỉ áp dụng unique cho các bản ghi có IsDefault = true
            // -- Giúp tránh race condition khi nhiều request cùng set default
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ux_address_user_default
                ON ""Addresses"" (""UserId"")
                WHERE ""IsDefault"" = true;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Addresses");
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ux_addresses_user_default;
            ");
        }
    }
}
