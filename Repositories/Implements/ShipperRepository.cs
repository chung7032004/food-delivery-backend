using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implements
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly FoodContext _context;

        public ShipperRepository(FoodContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderId)
        {
            return await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == orderId);
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }

        public async Task AddStatusHistoryAsync(OrderStatusHistory history)
        {
            await _context.OrderStatusHistories.AddAsync(history);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}