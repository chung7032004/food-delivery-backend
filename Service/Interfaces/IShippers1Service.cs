using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;

namespace FoodDelivery.Service.Interfaces;
public interface IShippers1Service
{
    Task<Result<PagedResponse<ShipperResponse>>> GetAllShippersAsync(int page, int pageSize);
    Task<Result<PagedResponse<OrderHistoryItemResponse>>> GetOrderByShipperIdAsync(Guid shipperId, int page, int pageSize);
    Task<Result<ShipperResponse>> GetByIdAsync(Guid id);
    Task<Result> RegisterShipperAsync(Guid userId);
    Task<Result> UpdateStatusAsync(Guid shipperId, bool IsActive);
}