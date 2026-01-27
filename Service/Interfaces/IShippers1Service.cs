using FoodDelivery.Common;

namespace FoodDelivery.Service.Interfaces;
public interface IShippers1Service
{
    Task<Result<PagedResponse<ShipperResponse>>> GetAllShippersAsync(int page, int pageSize);
    Task<Result<ShipperResponse>> GetByIdAsync(Guid id);
}