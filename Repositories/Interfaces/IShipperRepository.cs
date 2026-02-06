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
        Task<Shipper?> GetShipperByIdAsync(Guid userId);
        Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId);
        Task<List<OrderDetail>> GetShipperCompletedOrdersAsync(Guid shipperId);
        Task<List<OrderDetail>> GetAvailableOrdersAsync();
        Task AddShipperAsync(Shipper shipper);
        Task<decimal> GetTodayEarningsAsync(Guid shipperId);
        Task<decimal> GetTotalEarningsAsync(Guid shipperId);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddUserRoleAsync(UserRole userRole);
    }
}
        