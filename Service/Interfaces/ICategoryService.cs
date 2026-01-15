using FoodDelivery.DTOs.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    Task<CategoryResponseDto?> GetByIdAsync(Guid id);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
    Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}


