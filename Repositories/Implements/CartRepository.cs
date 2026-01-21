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
    public async Task<Cart?> GetCartWithItemsByCustomerIdAsync(Guid customerId,int page,int pageSize)
    {
        return await _context.Carts
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .Select(c => new Cart
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                CartItems = c.CartItems
                    .OrderBy(ci => ci.AddedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ci => new CartItem
                    {
                        Id = ci.Id,
                        Quantity = ci.Quantity,
                        ProductId = ci.ProductId,
                        AddedAt = ci.AddedAt,
                        UpdatedAt = ci.UpdatedAt,
                        Product = new Product
                        {
                            Id = ci.Product.Id,
                            Name = ci.Product.Name,
                            Price = ci.Product.Price,
                            ImageUrl = ci.Product.ImageUrl,
                            Category = new Category
                            {
                                Id = ci.Product.Category.Id,
                                Name = ci.Product.Category.Name
                            }
                        }
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
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