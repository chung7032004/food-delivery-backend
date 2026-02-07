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
            if (orderDetail == null || orderDetail.Status != OrderStatus.ReadyForPickup) return false;

            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null || orderDetail.ShipperId != shipper.Id) return false;

            orderDetail.Status = OrderStatus.Shipping;
            
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory { 
                OrderId = orderId, Status = OrderStatus.Shipping, ChangeByUserId = userId, ChangedAt = DateTime.UtcNow, ActionBy = OrderActionBy.Shipper
            });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<object> GetShipperStatsAsync(Guid userId)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null) 
                return new { totalDeliveries = 0, completedDeliveries = 0, pendingDeliveries = 0, deliveryInProgress = 0, earningToday = 0, averageRating = 5.0 };

            var history = await _shipperRepository.GetShipperHistoryAsync(userId);
            var completedCount = history.Count(h => h.Status == OrderStatus.Completed);
            
            // Tính đơn đang giao: đơn có status Shipping mà chưa Completed
            var shippingOrderIds = history
                .Where(h => h.Status == OrderStatus.Shipping && h.ChangeByUserId == userId)
                .Select(h => h.OrderId)
                .Distinct()
                .ToList();
            
            var completedOrderIds = history
                .Where(h => h.Status == OrderStatus.Completed)
                .Select(h => h.OrderId)
                .ToHashSet();
            
            var shippingCount = shippingOrderIds.Count(id => !completedOrderIds.Contains(id));
            
            // pendingDeliveries should show total available orders in system
            var availableOrders = await _shipperRepository.GetAvailableOrdersAsync();
            var readyForPickupCount = availableOrders.Count;
            
            var totalEarnings = await _shipperRepository.GetTotalEarningsAsync(shipper.Id);
            
            return new 
            {
                totalDeliveries = completedCount,
                completedDeliveries = completedCount,
                pendingDeliveries = readyForPickupCount,
                deliveryInProgress = shippingCount,
                earningToday = totalEarnings,
                averageRating = 5.0
            };
        }

       public async Task<List<OrderAdminSummaryResponse>> GetAssignedOrdersAsync(Guid userId)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper?.Orders == null || shipper.Orders.Count == 0) 
                return new List<OrderAdminSummaryResponse>();

            return shipper.Orders
                .Where(o => o.Status == OrderStatus.ReadyForPickup || o.Status == OrderStatus.Shipping)
                .OrderByDescending(o => o.Order?.CreatedAt)
                .Select(o => new OrderAdminSummaryResponse
                {
                    Id = o.OrderId,
                    OrderCode = o.Order?.OrderCode ?? o.OrderId.ToString().Substring(0, 8),
                    CustomerName = o.Order?.Customer?.FullName ?? "Khách hàng",
                    TotalAmount = o.Order?.TotalAmount ?? 0,
                    Status = o.Status.ToString(),
                    CreatedAt = o.Order?.CreatedAt ?? DateTime.UtcNow
                }).ToList();
        }

        public async Task<List<OrderAdminSummaryResponse>> GetAvailableOrdersAsync()
        {
            var availableOrders = await _shipperRepository.GetAvailableOrdersAsync();
            return availableOrders
                .Select(o => new OrderAdminSummaryResponse
                {
                    Id = o.OrderId,
                    OrderCode = o.Order?.OrderCode ?? o.OrderId.ToString().Substring(0, 8),
                    CustomerName = o.Order?.Customer?.FullName ?? "Khách hàng",
                    TotalAmount = o.Order?.TotalAmount ?? 0,
                    Status = o.Status.ToString(),
                    CreatedAt = o.Order?.CreatedAt ?? DateTime.UtcNow
                }).ToList();
        }

        public async Task<List<OrderAdminSummaryResponse>> GetShipperCompletedOrdersAsync(Guid userId)
        {
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null)
                return new List<OrderAdminSummaryResponse>();

            var completedOrders = await _shipperRepository.GetShipperCompletedOrdersAsync(shipper.Id);
            return completedOrders
                .Select(o => new OrderAdminSummaryResponse
                {
                    Id = o.OrderId,
                    OrderCode = o.Order?.OrderCode ?? o.OrderId.ToString().Substring(0, 8),
                    CustomerName = o.Order?.Customer?.FullName ?? "Khách hàng",
                    TotalAmount = o.Order?.TotalAmount ?? 0,
                    Status = o.Status.ToString(),
                    CreatedAt = o.Order?.CreatedAt ?? DateTime.UtcNow
                }).ToList();
        }

        public async Task<bool> AcceptOrderAsync(Guid orderId, Guid shipperId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
                if (orderDetail == null)
                    throw new InvalidOperationException("Không tìm thấy đơn hàng");

                if (orderDetail.Status != OrderStatus.ReadyForPickup)
                    throw new InvalidOperationException("Đơn hàng không ở trạng thái sẵn sàng để nhận");
            
            // Ensure the order is not yet assigned to anyone
                if (orderDetail.ShipperId != null && orderDetail.ShipperId != Guid.Empty)
                    throw new InvalidOperationException("Đơn hàng đã được nhận bởi shipper khác");

            // Get Shipper entity by UserId to get the correct Shipper.Id for FK
            var shipper = await _shipperRepository.GetShipperByIdAsync(shipperId);
            if (shipper == null)
            {
                // If a Shipper record does not exist yet, create one for this user
                shipper = new Shipper
                {
                    UserId = shipperId,
                    IsActive = true,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _shipperRepository.AddShipperAsync(shipper);
            }

            orderDetail.ShipperId = shipper.Id;
            _shipperRepository.UpdateOrderDetail(orderDetail);
            
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory 
            { 
                OrderId = orderId, 
                Status = OrderStatus.ReadyForPickup, 
                ChangeByUserId = shipperId, 
                ChangedAt = DateTime.UtcNow, 
                ActionBy = OrderActionBy.Shipper,
                Note = "Shipper đã nhận đơn hàng"
            });
            
            return await _shipperRepository.SaveChangesAsync();
        }
        
        public async Task<OrderDetailResponse?> GetOrderByIdAsync(Guid orderId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail?.Order == null) return null;

            return new OrderDetailResponse
            {
                OrderId = orderDetail.OrderId,
                OrderCode = orderDetail.Order.OrderCode,
                CreatedAt = orderDetail.Order.CreatedAt,
                CustomerId = orderDetail.Order.CustomerId,
                CustomerName = orderDetail.Order.Customer?.FullName ?? "Khách hàng",
                CustomerEmail = orderDetail.Order.Customer?.Email ?? string.Empty,
                ShippingAddress = orderDetail.Order.ShippingAddress,
                ReceiverName = orderDetail.Order.ReceiverName,
                ReceiverPhone = orderDetail.Order.ReceiverPhone,
                ShippingFee = orderDetail.Order.ShippingFee,
                EstimatedDeliveryTime = orderDetail.EstimatedDeliveryTime,
                ActualDeliveryTime = orderDetail.ActualDeliveryTime,
                PaymentMethod = orderDetail.PaymentMethod.ToString(),
                PaymentStatus = orderDetail.PaymentStatus.ToString(),
                CurrentStatus = orderDetail.Status.ToString(),
                CancelReason = orderDetail.CancelReason,
                TotalAmount = orderDetail.Order.TotalAmount,
                ShipperId = orderDetail.ShipperId,
                Items = orderDetail.Order.OrderItems?.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.ProductImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.UnitPrice * i.Quantity,
                    IsRemoved = i.IsRemoved,
                    RemoveReason = i.RemoveReason
                }).ToList() ?? new List<OrderItemDTO>(),
                StatusHistories = orderDetail.Order.OrderStatusHistories?.Select(h => new OrderStatusHistoryDTO
                {
                    Status = h.Status.ToString(),
                    ChangedAt = h.ChangedAt,
                    ChangedBy = h.ChangeByUserId,
                    Note = h.Note
                }).OrderByDescending(h => h.ChangedAt).ToList() ?? new List<OrderStatusHistoryDTO>()
            };
        }

        

        public async Task<bool> MarkSuccessAsync(Guid orderId, Guid userId)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null) return false;
            
            var shipper = await _shipperRepository.GetShipperByIdAsync(userId);
            if (shipper == null || orderDetail.ShipperId != shipper.Id) return false;

            // Allow from both ReadyForPickup and Shipping statuses
            if (orderDetail.Status != OrderStatus.ReadyForPickup && orderDetail.Status != OrderStatus.Shipping) 
                return false;

            // Auto confirm pickup if still in ReadyForPickup status
            if (orderDetail.Status == OrderStatus.ReadyForPickup)
            {
                orderDetail.Status = OrderStatus.Shipping;
                await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory 
                { 
                    OrderId = orderId, 
                    Status = OrderStatus.Shipping, 
                    ChangeByUserId = userId,
                    ActionBy = OrderActionBy.Shipper,
                    ChangedAt = DateTime.UtcNow,
                    Note = "Xác nhận lấy hàng từ quán"
                });
            }

            orderDetail.Status = OrderStatus.Completed;
            orderDetail.ActualDeliveryTime = DateTime.UtcNow;

            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory 
            { 
                OrderId = orderId, 
                Status = OrderStatus.Completed, 
                ChangeByUserId = userId,
                ActionBy = OrderActionBy.Shipper,
                ChangedAt = DateTime.UtcNow,
                Note = "Giao hàng thành công"
            });
            return await _shipperRepository.SaveChangesAsync();
        }

        public async Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null)
        {
            var orderDetail = await _shipperRepository.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null || !cancelledBy.HasValue) return false;
            
            var shipper = await _shipperRepository.GetShipperByIdAsync(cancelledBy.Value);
            if (shipper == null || orderDetail.ShipperId != shipper.Id) return false;

            // Allow from both ReadyForPickup and Shipping statuses
            if (orderDetail.Status != OrderStatus.ReadyForPickup && orderDetail.Status != OrderStatus.Shipping)
                return false;

            // Auto confirm pickup if still in ReadyForPickup status
            if (orderDetail.Status == OrderStatus.ReadyForPickup)
            {
                orderDetail.Status = OrderStatus.Shipping;
                await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory 
                { 
                    OrderId = orderId, 
                    Status = OrderStatus.Shipping, 
                    ChangeByUserId = cancelledBy.Value,
                    ActionBy = OrderActionBy.Shipper,
                    ChangedAt = DateTime.UtcNow,
                    Note = "Xác nhận lấy hàng từ quán"
                });
            }
            
            orderDetail.Status = OrderStatus.Cancelled;
            orderDetail.CancelReason = reason ?? string.Empty;
            orderDetail.CancelledByUserId = cancelledBy.Value;
            
            _shipperRepository.UpdateOrderDetail(orderDetail);
            await _shipperRepository.AddStatusHistoryAsync(new OrderStatusHistory 
            { 
                OrderId = orderId, 
                Status = OrderStatus.Cancelled, 
                ChangeByUserId = cancelledBy.Value,
                ActionBy = OrderActionBy.Shipper,
                ChangedAt = DateTime.UtcNow,
                Note = reason ?? "Giao hàng thất bại"
            });
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
            shipper.UpdatedAt = DateTime.UtcNow;

            return await _shipperRepository.SaveChangesAsync();
        }
        
    }
}
