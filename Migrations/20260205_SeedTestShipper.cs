using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestShipper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var shipperId = "cccccccc-cccc-cccc-cccc-cccccccccccc";
            var shipperRoleId = "10000000-0000-0000-0000-000000000002";

            // Insert Shipper record using raw SQL
            migrationBuilder.Sql(
                $@"INSERT INTO ""Shippers"" (""Id"", ""UserId"", ""IsActive"", ""IsAvailable"", ""TotalDeliveredOrders"", ""CreatedAt"")
                   VALUES ('{Guid.NewGuid()}', '{shipperId}', true, true, 0, now())
                   ON CONFLICT DO NOTHING;"
            );

            // Assign Shipper role to the user - only if role exists
            migrationBuilder.Sql(
                $@"INSERT INTO ""UserRoles"" (""UserId"", ""RoleId"")
                   SELECT '{shipperId}', id FROM ""Roles""
                   WHERE ""Name"" = 'Shipper'
                   AND NOT EXISTS (
                       SELECT 1 FROM ""UserRoles""
                       WHERE ""UserId"" = '{shipperId}'
                   );"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var shipperId = "cccccccc-cccc-cccc-cccc-cccccccccccc";

            // Delete shipper role assignment
            migrationBuilder.Sql(
                $@"DELETE FROM ""UserRoles""
                   WHERE ""UserId"" = '{shipperId}';"
            );

            // Delete shipper record
            migrationBuilder.Sql(
                $@"DELETE FROM ""Shippers"" WHERE ""UserId"" = '{shipperId}';"
            );
        }
    }
}
