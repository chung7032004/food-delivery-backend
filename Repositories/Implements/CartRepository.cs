using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class CartRepository : ICartRepository
{
    private readonly FoodContext _context;
    public CartRepository(FoodContext context)
    {
        _context = context;
    }
    public async Task<Cart?> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Carts
            .FirstOrDefaultAsync(c=>c.CustomerId == customerId);
    }
    public async Task<Cart?> GetCartWithItemsByCustomerIdAsync(Guid customerId)
    {
        return await _context.Carts.AsNoTracking()
                .Include(c=>c.CartItems)
                .ThenInclude(ci=>ci.Product)
                .ThenInclude(p=>p.Category)
                .FirstOrDefaultAsync(c=>c.CustomerId == customerId);
    }
    public async Task<Cart?> GetByIdAsync(Guid cartId)
    {
        return await _context.Carts.FindAsync(cartId);
    }
    public async Task AddAsync (Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }
    public Task UpdateAsync (Cart cart)
    {
        _context.Carts.Update(cart);
        return Task.CompletedTask;
    }
    public Task DeleteAsync (Cart cart)
    {
        _context.Carts.Remove(cart);
        return Task.CompletedTask;
    }
}