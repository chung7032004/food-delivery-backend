using FoodDelivery.DTOs.Product;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepo,
            IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductResponeDto>> GetAllProductsAsync(Guid? categoryId = null, string? searchQuery = null)
        {
            var products = await _productRepo.GetAllAsync();
            
            // Filter by category if provided
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            
            // Filter by search query if provided
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerQuery = searchQuery.ToLower();
                products = products.Where(p => 
                    p.Name.ToLower().Contains(lowerQuery) || 
                    (p.Category != null && p.Category.Name.ToLower().Contains(lowerQuery))
                ).ToList();
            }
            
            return products
                .OrderBy(p => p.DisplayOrder)
                .Select(MapToDto);
        }

        public async Task<IEnumerable<ProductResponeDto>> GetFeaturedProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products
                .Where(p => p.IsFeatured && p.IsAvailable)
                .OrderBy(p => p.DisplayOrder)
                .Select(MapToDto);
        }

        public async Task<ProductResponeDto?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductResponeDto> CreateProductAsync(ProductCreateDto dto)
        {
            // Get all products to calculate next display order
            var allProducts = await _productRepo.GetAllAsync();
            
            // If displayOrder is not provided or is 0, calculate it automatically
            int displayOrder = dto.DisplayOrder > 0 ? dto.DisplayOrder : allProducts.Count();
            
            var product = new Product
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                IsAvailable = dto.IsAvailable,
                IsFeatured = dto.IsFeatured,
                DisplayOrder = displayOrder,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> UpdateProductAsync(Guid productId, ProductUpdateDto dto)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl ?? string.Empty;
            product.IsAvailable = dto.IsAvailable;
            product.IsFeatured = dto.IsFeatured;
            product.DisplayOrder = dto.DisplayOrder;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDisplayOrderAsync(Guid productId, int newDisplayOrder)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

            product.DisplayOrder = newDisplayOrder;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleProductAvailabilityAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

            product.IsAvailable = !product.IsAvailable;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

            // Delete the product
            await _productRepo.DeleteAsync(product);
            
            // Get remaining products sorted by display order
            var remainingProducts = await _productRepo.GetAllAsync();
            var sortedProducts = remainingProducts.Where(p => p.Id != productId).OrderBy(p => p.DisplayOrder).ToList();
            
            // Reassign display order sequentially
            for (int i = 0; i < sortedProducts.Count; i++)
            {
                sortedProducts[i].DisplayOrder = i;
                await _productRepo.UpdateAsync(sortedProducts[i]);
            }
            
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static ProductResponeDto MapToDto(Product p)
        {
            return new ProductResponeDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "",
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable,
                IsFeatured = p.IsFeatured,
                DisplayOrder = p.DisplayOrder,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
