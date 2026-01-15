using FoodDelivery.Entities;
using Microsoft.EntityFrameworkCore;

public class CategoryRepository : ICategoryRepository
{
    private readonly FoodContext _context;

    public CategoryRepository(FoodContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id)
        => await _context.Categories.FindAsync(id);

    public async Task AddAsync(Category category)
        => await _context.Categories.AddAsync(category);

    public void Update(Category category)
        => _context.Categories.Update(category);

    public void Delete(Category category)
        => _context.Categories.Remove(category);
}
