using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(Guid customerId);
    Task<Cart?> GetCartWithItemsByCustomerIdAsync(Guid customerId);
    Task<Cart?> GetByIdAsync(Guid cartId);
    Task AddAsync (Cart cart);
    Task UpdateAsync (Cart cart);
    Task DeleteAsync (Cart cart);
}