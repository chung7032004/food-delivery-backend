using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IShipperRepository
    {
        Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderId);
        void UpdateOrderDetail(OrderDetail orderDetail);
        Task AddStatusHistoryAsync(OrderStatusHistory history);
        Task<bool> SaveChangesAsync();
        Task<List<User>> GetAllShippersAsync();
        Task<User?> GetShipperByIdAsync(Guid userId);
        Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId);
        
        // 2 hàm quan trọng để hết lỗi đỏ CS1061
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddUserRoleAsync(UserRole userRole);
    }
}
        