using FoodDelivery.DTOs.Product;    
namespace FoodDelivery.Service.Implements
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponeDto>> GetAllProductsAsync(Guid? categoryId = null, string? searchQuery = null);
        Task<IEnumerable<ProductResponeDto>> GetFeaturedProductsAsync();
        Task<ProductResponeDto?> GetProductByIdAsync(Guid productId);
        Task<ProductResponeDto> CreateProductAsync(ProductCreateDto productCreateDto);
        Task<bool> UpdateProductAsync(Guid productId, ProductUpdateDto productUpdateDto);
        Task<bool> UpdateDisplayOrderAsync(Guid productId, int newDisplayOrder);
        Task<bool> ToggleProductAvailabilityAsync(Guid productId);
        Task<bool> DeleteProductAsync(Guid productId);
    }
}