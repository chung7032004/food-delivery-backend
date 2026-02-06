using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implements
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly FoodContext _context;
        public ShipperRepository(FoodContext context) { _context = context; }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderId) => 
            await _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.OrderItems)
                .Include(od => od.Order)
                    .ThenInclude(o => o.OrderStatusHistories)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .FirstOrDefaultAsync(od => od.OrderId == orderId);

        public void UpdateOrderDetail(OrderDetail orderDetail) => _context.OrderDetails.Update(orderDetail);

        public async Task AddStatusHistoryAsync(OrderStatusHistory history) => await _context.OrderStatusHistories.AddAsync(history);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public async Task<List<User>> GetAllShippersAsync() => 
            await _context.Users.Where(u => u.UserRoles.Any(r => r.Role.Name == "Shipper")).ToListAsync();

        public async Task<Shipper?> GetShipperByIdAsync(Guid userId)
        {
            return await _context.Shippers
                .Include(s => s.User)
                .Include(s => s.Orders)
                    .ThenInclude(o => o.Order)
                        .ThenInclude(o => o.Customer)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
            
        public async Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId) => 
            await _context.OrderStatusHistories.Where(h => h.ChangeByUserId == userId).OrderByDescending(h => h.ChangedAt).ToListAsync();

        public async Task<List<OrderDetail>> GetShipperCompletedOrdersAsync(Guid shipperId) =>
            await _context.OrderDetails
                .Where(od => od.ShipperId == shipperId && od.Status == OrderStatus.Completed)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .OrderByDescending(od => od.Order.CreatedAt)
                .ToListAsync();

        public async Task<List<OrderDetail>> GetAvailableOrdersAsync()
        {
            return await _context.OrderDetails
                .Where(od => od.Status == OrderStatus.ReadyForPickup && od.ShipperId == null)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .OrderByDescending(od => od.Order.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTodayEarningsAsync(Guid shipperId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.OrderDetails
                .Where(od => od.ShipperId == shipperId 
                    && od.Status == OrderStatus.Completed
                    && od.Order.CreatedAt.Date == today)
                .SumAsync(od => od.Order.TotalAmount);
        }

        public async Task<decimal> GetTotalEarningsAsync(Guid shipperId)
        {
            return await _context.OrderDetails
                .Where(od => od.ShipperId == shipperId && od.Status == OrderStatus.Completed)
                .SumAsync(od => od.Order.TotalAmount);
        }

        public async Task AddShipperAsync(Shipper shipper) => await _context.Shippers.AddAsync(shipper);

        public async Task<Role?> GetRoleByNameAsync(string roleName) => 
            await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        public async Task AddUserRoleAsync(UserRole userRole) => await _context.UserRoles.AddAsync(userRole);
    }
}