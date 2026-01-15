using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

<<<<<<< HEAD
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
=======
namespace FoodDelivery.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly FoodContext _context;

        public ProductRepository(FoodContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }
    }
}
>>>>>>> 5ab51ab (Add category crud & category migration and fix product flow)
