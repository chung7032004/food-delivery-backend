using FoodDelivery.Entities;
using FoodDelivery.DTOs;
using FoodDelivery.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Service.Implements
{
    public class ShipperService : IShipperService
    {
        private readonly FoodContext _context;
        public ShipperService(FoodContext context) { _context = context; }
        public async Task<bool> ConfirmPickUpAsync(Guid orderId, Guid userId)
        {
            var detail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (detail == null)
                return false;

            detail.ShipperId = userId;
            detail.Status = OrderStatus.Shipping;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkSuccessAsync(Guid orderId)
        {
            var detail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (detail == null)
                return false;

            detail.Status = OrderStatus.Completed;
            detail.ActualDeliveryTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null)
        {
            var detail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (detail == null)
                return false;

            detail.Status = OrderStatus.Cancelled;
            detail.CancelReason = reason ?? string.Empty;
            if (cancelledBy.HasValue)
                detail.CancelledByUserId = cancelledBy.Value;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}