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
            if (orderDetail == null) return false;
            orderDetail.Status = OrderStatus.Shipping;
            orderDetail.ShipperId = userId;
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { OrderId = orderId, Status = OrderStatus.Shipping, ChangeByUserId = userId, ChangedAt = DateTime.Now });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> MarkSuccessAsync(Guid orderId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null) return false;
            orderDetail.Status = OrderStatus.Completed;
            orderDetail.ActualDeliveryTime = DateTime.Now;
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { OrderId = orderId, Status = OrderStatus.Completed, ChangedAt = DateTime.Now });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null) return false;
            orderDetail.Status = OrderStatus.Cancelled;
            orderDetail.CancelReason = reason ?? string.Empty;
            if (cancelledBy.HasValue) orderDetail.CancelledByUserId = cancelledBy.Value;
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { OrderId = orderId, Status = OrderStatus.Cancelled, ChangeByUserId = cancelledBy ?? Guid.Empty, ChangedAt = DateTime.Now });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllShippersAsync() => await _shipperRepository.GetAllShippersAsync();
        public async Task<User?> GetShipperByIdAsync(Guid userId) => await _shipperRepository.GetShipperByIdAsync(userId);
        public async Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId) => await _shipperRepository.GetShipperHistoryAsync(userId);

        public async Task<bool> ToggleShipperStatusAsync(Guid userId, bool isActive)
        {
            var user = await _shipperRepository.GetShipperByIdAsync(userId);
            if (user == null) return false;
            user.IsActive = isActive;
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> AssignShipperRoleAsync(Guid userId)
        {
            var role = await _shipperRepository.GetRoleByNameAsync("Shipper");
            if (role == null) return false;
            await _shipperRepository.AddUserRoleAsync(new UserRole { UserId = userId, RoleId = role.Id });
            return await _shipperRepository.SaveChangesAsync();
        }
    }
}