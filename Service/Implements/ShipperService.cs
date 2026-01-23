using FoodDelivery.Entities;
using FoodDelivery.DTOs;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.Repositories.Interfaces;

namespace FoodDelivery.Service.Implements
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;
        public ShipperService(IShipperRepository shipperRepository) { _shipperRepository = shipperRepository; }
        public async Task<bool> ConfirmPickUpAsync(Guid orderId, Guid userId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null)
                return false;

            // Cập nhật thông tin shipper và trạng thái
            orderDetail.Status = OrderStatus.Shipping;
            orderDetail.ShipperId = userId;
            _shipperRepository.UpdateOrderDetail(orderDetail);

            // Lưu lịch sử thay đổi trạng thái
            var history = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Shipping,
                ChangeByUserId = userId,
                ChangedAt = DateTime.Now
            };
            await _shipperRepository.AddStatusHistoryAsync(history);

            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> MarkSuccessAsync(Guid orderId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null)
                return false;

            orderDetail.Status = OrderStatus.Completed;
            orderDetail.ActualDeliveryTime = DateTime.Now;
            _shipperRepository.UpdateOrderDetail(orderDetail);

            // Lưu lịch sử thay đổi trạng thái
            var history = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Completed,
                ChangedAt = DateTime.Now
            };
            await _shipperRepository.AddStatusHistoryAsync(history);

            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null)
                return false;

            orderDetail.Status = OrderStatus.Cancelled;
            orderDetail.CancelReason = reason ?? string.Empty;
            if (cancelledBy.HasValue)
                orderDetail.CancelledByUserId = cancelledBy.Value;
            _shipperRepository.UpdateOrderDetail(orderDetail);

            // Lưu lịch sử thay đổi trạng thái
            var history = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Cancelled,
                ChangeByUserId = cancelledBy ?? Guid.Empty,
                ChangedAt = DateTime.Now
            };
            await _shipperRepository.AddStatusHistoryAsync(history);

            return await _shipperRepository.SaveChangesAsync();
        }
    }
}