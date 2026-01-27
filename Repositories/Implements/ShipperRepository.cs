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
            await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == orderId);

        public void UpdateOrderDetail(OrderDetail orderDetail) => _context.OrderDetails.Update(orderDetail);

        public async Task AddStatusHistoryAsync(OrderStatusHistory history) => await _context.OrderStatusHistories.AddAsync(history);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public async Task<List<User>> GetAllShippersAsync() => 
            await _context.Users.Where(u => u.UserRoles.Any(r => r.Role.Name == "Shipper")).ToListAsync();

        public async Task<User?> GetShipperByIdAsync(Guid userId) => 
            await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId) => 
            await _context.OrderStatusHistories.Where(h => h.ChangeByUserId == userId).OrderByDescending(h => h.ChangedAt).ToListAsync();

        public async Task<Role?> GetRoleByNameAsync(string roleName) => 
            await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        public async Task AddUserRoleAsync(UserRole userRole) => await _context.UserRoles.AddAsync(userRole);
    }
}