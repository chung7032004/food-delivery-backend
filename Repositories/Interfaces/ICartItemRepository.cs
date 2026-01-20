using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface ICartItemRepository
{
    Task<CartItem?> GetByIdAsync(Guid id);
    Task<List<CartItem>> GetByIdsAsync(Guid customerId,List<Guid> ids);
    Task<CartItem?> GetByCartIdAndProductIdAsync(Guid cartId, Guid productId);
    Task<List<CartItem>> GetByCartIdAsync(Guid cartId);
    Task AddAsync(CartItem item);
    Task UpdateAsync(CartItem item);
    Task DeleteAsync(CartItem item);
    Task DeleteRangeAsync(IEnumerable<CartItem> items);
}