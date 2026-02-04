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
            if (orderDetail == null || orderDetail.Status != OrderStatus.Pending) return false;

            orderDetail.Status = OrderStatus.Shipping;
            orderDetail.ShipperId = userId;
            
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { 
                OrderId = orderId, Status = OrderStatus.Shipping, ChangeByUserId = userId, ChangedAt = DateTime.Now 
            });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<object> GetShipperStatsAsync(Guid userId)
        {
           
            var history = await _shipperRepository.GetShipperHistoryAsync(userId);
            
            
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            
           
            var totalCompleted = history.Count(h => h.Status == OrderStatus.Completed);
            var inProgress = shipper?.Orders?.Count(o => o.Status == OrderStatus.Shipping) ?? 0;
            var pendingCount = shipper?.Orders?.Count(o => o.Status == OrderStatus.Pending) ?? 0;
            
            return new 
            {
                totalDeliveries = totalCompleted,
                completedDeliveries = totalCompleted,
                pendingDeliveries = pendingCount,
                deliveryInProgress = inProgress,
                earningToday = 0, // Cần truy vấn thêm doanh thu nếu cần
                averageRating = 5.0
            };
        }

       public async Task<List<OrderAdminSummaryResponse>> GetAssignedOrdersAsync(Guid userId)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null || shipper.Orders == null) return new List<OrderAdminSummaryResponse>();

            return shipper.Orders.Select(o => new OrderAdminSummaryResponse
            {
                Id = o.OrderId,
                OrderCode = o.Order?.OrderCode ?? o.OrderId.ToString().Substring(0, 8),
                CustomerName = o.Order?.Customer?.FullName ?? "Khách hàng",
                TotalAmount = o.Order?.TotalAmount ?? 0,
                Status = o.Status.ToString(),
                CreatedAt = o.Order?.CreatedAt ?? DateTime.Now
            }).ToList();
        }
        
        public async Task<OrderDetailResponse?> GetOrderByIdAsync(Guid orderId)
        {
            
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            
            if (orderDetail == null || orderDetail.Order == null) return null;

            
            return new OrderDetailResponse
            {
                OrderId = orderDetail.OrderId,
                OrderCode = orderDetail.Order.OrderCode,
                Status = orderDetail.Status.ToString(),
                CustomerName = orderDetail.Order.Customer?.FullName ?? "Khách hàng",
                PhoneNumber = orderDetail.Order.ReceiverPhone ?? "Không có số điện thoại", 
                DeliveryAddress = orderDetail.Order.ShippingAddress ?? "Không có địa chỉ",
                TotalAmount = orderDetail.Order.TotalAmount,
                CreatedAt = orderDetail.Order.CreatedAt
                
            };
        }

        

        public async Task<bool> MarkSuccessAsync(Guid orderId)
{
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            // Kiểm tra trạng thái trước khi đổi
            if (orderDetail == null || orderDetail.Status != OrderStatus.Shipping) return false;

            orderDetail.Status = OrderStatus.Completed;
            orderDetail.ActualDeliveryTime = DateTime.Now;

            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { 
                OrderId = orderId, Status = OrderStatus.Completed, ChangedAt = DateTime.Now 
            });
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
        public async Task<Shipper?> GetShipperByIdAsync(Guid userId) => await _shipperRepository.GetShipperByIdAsync(userId);
        public async Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId) => await _shipperRepository.GetShipperHistoryAsync(userId);

        public async Task<bool> ToggleShipperStatusAsync(Guid userId, bool isActive)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null || shipper.User == null) return false;

           
            shipper.User.IsActive = isActive; 

            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> AssignShipperRoleAsync(Guid userId)
        {
            var role = await _shipperRepository.GetRoleByNameAsync("Shipper");
            if (role == null) return false;
            await _shipperRepository.AddUserRoleAsync(new UserRole { UserId = userId, RoleId = role.Id });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateShipperProfileAsync(Guid userId, UpdateShipperProfileDto request)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null) return false;

            if (request.IsAvailable.HasValue)
            {
                shipper.IsAvailable = request.IsAvailable.Value;
            }
            shipper.UpdatedAt = DateTime.Now;

            return await _shipperRepository.SaveChangesAsync();
        }
        
    }
}
