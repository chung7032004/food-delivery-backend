using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class OrderRepository : IOrderRepository
{
    private readonly FoodContext _context;
    public OrderRepository (FoodContext context)
    {
        _context = context;
    }
    public async Task<List<OrderHistoryItemResponse>> GetMyOrderAsync(Guid customerId, int page, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o=> o.CustomerId == customerId)
            .OrderByDescending(o=>o.CreatedAt)
            .Skip((page-1)*pageSize)
            .Take(pageSize)
            .Select(o=> new OrderHistoryItemResponse
            {
                OrderId = o.Id,
                OrderCode = o.OrderCode,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                ShippingFee = o.ShippingFee,
                CurrentStatus = o.OrderDetail.Status,
                EstimatedDeliveryTime = o.OrderDetail.EstimatedDeliveryTime
            })
            .ToListAsync();
    }
    public async Task<Order?> GetOrderDetailAsync(Guid orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o=>o.Customer)
            .Include(o=>o.OrderDetail)
                .ThenInclude(od=>od.Shipper)
            .Include(o=>o.OrderDetail)
                .ThenInclude(od=>od.CancelledByUser)
            .Include(o=>o.OrderItems)
            .Include(o=>o.OrderStatusHistories)
                .ThenInclude(h=>h.ChangeByUser)
            .FirstOrDefaultAsync(o=>orderId == o.Id);
    }
    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await _context.Orders
            .Include(o=>o.OrderDetail)
            .Include(o=>o.OrderItems)
            .FirstOrDefaultAsync(o=>o.Id == orderId);
    }
    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }
    public async Task<int> GetTotalCountAsync(Guid customerId)
    {
        return await _context.Orders.CountAsync(o=>o.CustomerId == customerId);
    }
    public async Task AddOrderHistoryAsync(OrderStatusHistory orderStatusHistory)
    {
        await _context.OrderStatusHistories.AddAsync(orderStatusHistory);
    }
    public IQueryable<Order> GetQueryable()
    {
        return _context.Orders.AsNoTracking().Include(o=>o.Customer).Include(o=>o.OrderDetail);
    }
}