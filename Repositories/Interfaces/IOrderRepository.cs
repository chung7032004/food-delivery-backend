using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface IOrderRepository
{
    Task<List<OrderHistoryItemResponse>> GetMyOrderAsync(Guid customerId, int page, int pageSize);
    Task<Order?> GetOrderDetailAsync(Guid orderId);
    Task<Order?> GetOrderById(Guid orderId);
    Task AddAsync(Order order);
    Task<int> GetTotalCountAsync(Guid customerId);
    Task AddOrderHistoryAsync(OrderStatusHistory orderStatusHistory);
    IQueryable<Order> GetQueryable();
}