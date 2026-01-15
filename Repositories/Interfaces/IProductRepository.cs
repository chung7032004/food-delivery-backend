using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId);
    Task<bool> IsAvailableAsync(Guid productId);
    Task<bool> ExistsAsync(Guid productId);
}