using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface IShippers1Repository
{
    Task<(List<Shipper> Data , int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Shipper?> GetByIdAsync(Guid id);
    Task<Shipper?> GetByUserIdAsync(Guid userId);
    Task<Shipper?> GetByIdWithUser(Guid id);
    Task AddAsync(Shipper shipper);
    Task UpdateAsync (Shipper shipper);
    Task DeleteAsync (Shipper shipper);
}