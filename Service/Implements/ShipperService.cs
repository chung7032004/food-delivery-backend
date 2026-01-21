using FoodDelivery.Entities;
using FoodDelivery.DTOs;
using FoodDelivery.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Service.Implements
{
    public class ShipperService : IShipperService
    {
        //Viết repository cho service shipper 
        private readonly FoodContext _context;
        public ShipperService(FoodContext context) { _context = context; }
        public async Task<bool> ConfirmPickUpAsync(Guid orderId, Guid userId)
        {
            //Lấy shipperId thông qua userId,
            var detail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (detail == null)
                return false;
            if (detail.Status != OrderStatus.Preparing && detail.Status != OrderStatus.ReadyForPickup)
                return false;
            if (detail.ShipperId != null)
                return false;
            detail.ShipperId = userId; // cập nhật lại là shipperId cho đúng 
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
        //1. Xem danh sách đơn có thể nhận
        //2. Xem đơn đang giao của mình 
        
        //Admin
        //1. Tạo shipper từ user
        //2. Danh sách shipper
        //3. Xem chi tiết shipper
        //4. Kích hoạt / khóa shipper
        //5. Xem lịch sử giao hàng của shipper
    }
}