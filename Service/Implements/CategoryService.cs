using FoodDelivery.DTOs.Category;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations
{

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(
            ICategoryRepository repo,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
        var categories = await _repo.GetAllAsync();
        return categories.Select(MapToDto);
        }


        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            return category == null ? null : MapToDto(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.DisplayOrder = dto.DisplayOrder;
            category.UpdatedAt = DateTime.UtcNow;

            _repo.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return false;

            _repo.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static CategoryResponseDto MapToDto(Category c)
            => new()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DisplayOrder = c.DisplayOrder
            };
    }
}