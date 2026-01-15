using FoodDelivery.Entities;

<<<<<<< HEAD
namespace FoodDelivery.Repositories.Interfaces;
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId);
    Task<bool> IsAvailableAsync(Guid productId);
    Task<bool> ExistsAsync(Guid productId);
}
=======
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetByCategoryAsync(Guid categoryId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
>>>>>>> 5ab51ab (Add category crud & category migration and fix product flow)
