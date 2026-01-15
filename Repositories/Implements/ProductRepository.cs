using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class ProductRepository : IProductRepository
{
    private readonly FoodContext _context;
    public ProductRepository(FoodContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsAsync(Guid productId)
    {
        return await _context.Products.AnyAsync(p=>p.Id == productId);
    }
    public async Task<bool> IsAvailableAsync(Guid productId)
    {
        return await _context.Products.AnyAsync(p=>p.Id == productId && p.IsAvailable);
    }
    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _context.Products.FindAsync(productId);
    }
}