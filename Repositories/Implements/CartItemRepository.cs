using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class CartItemRepository : ICartItemRepository
{
    private readonly FoodContext _context;
    public CartItemRepository (FoodContext context)
    {
        _context = context;
    }
    public async Task<CartItem?> GetByIdAsync(Guid id)
    {
        return await _context.CartItems.FindAsync(id);
    }
    public async Task<CartItem?> GetByCartIdAndProductIdAsync(Guid cartId, Guid productId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(ci=> ci.CartId == cartId && ci.ProductId == productId);
    }
    public async Task<List<CartItem>> GetByCartIdAsync(Guid cartId)
    {
        return await _context.CartItems.AsNoTracking()
            .Include(ci=>ci.Product)
            .Where(ci=>ci.CartId==cartId)
            .ToListAsync();
    }
    public async Task AddAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
    }
    public Task UpdateAsync(CartItem item)
    {
        _context.CartItems.Update(item);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        return Task.CompletedTask;
    }
    public Task DeleteRangeAsync(IEnumerable<CartItem> items)
    {
        _context.CartItems.RemoveRange(items);
        return Task.CompletedTask;
    }
}