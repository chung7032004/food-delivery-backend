using FoodDelivery.DTOs.Product;
using FoodDelivery.Entities;
using FoodDelivery.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Service.Implements
{
    public class ProductService : IProductService
    {
        private readonly FoodContext _context;

        public ProductService(FoodContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponeDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.DisplayOrder)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductResponeDto>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsAvailable)
                .OrderBy(p => p.DisplayOrder)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<ProductResponeDto?> GetProductByIdAsync(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductResponeDto> CreateProductAsync(ProductCreateDto productCreateDto)
        {
            var product = new Product
            {
                CategoryId = productCreateDto.CategoryId,
                Name = productCreateDto.Name,
                Price = productCreateDto.Price,
                ImageUrl = productCreateDto.ImageUrl ?? string.Empty,
                IsAvailable = productCreateDto.IsAvailable,
                IsFeatured = productCreateDto.IsFeatured,
                DisplayOrder = productCreateDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id)
                   ?? throw new Exception("Cannot create product");
        }

        public async Task<bool> UpdateProductAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.Name = productUpdateDto.Name;
            product.Price = productUpdateDto.Price;
            product.ImageUrl = productUpdateDto.ImageUrl ?? string.Empty;
            product.IsAvailable = productUpdateDto.IsAvailable;
            product.IsFeatured = productUpdateDto.IsFeatured;
            product.DisplayOrder = productUpdateDto.DisplayOrder;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleProductAvailabilityAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsAvailable = !product.IsAvailable;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProductResponeDto MapToDto(Product p)
        {
            return new ProductResponeDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
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