using FoodDelivery.Entities;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category);
}
