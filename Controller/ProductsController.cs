using FoodDelivery.DTOs.Product;
using FoodDelivery.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

    namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] Guid? categoryId, [FromQuery] string? q)
            => Ok(await _productService.GetAllProductsAsync(categoryId, q));
        
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts()
            => Ok(await _productService.GetFeaturedProductsAsync());
        
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            return  product == null ? NotFound() : Ok(product);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
            => Ok(await _productService.CreateProductAsync(dto));

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateDto dto)
            => await _productService.UpdateProductAsync(id, dto) ? Ok() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/display-order")]
        public async Task<IActionResult> UpdateDisplayOrder(Guid id, [FromBody] UpdateDisplayOrderDto dto)
            => await _productService.UpdateDisplayOrderAsync(id, dto.NewDisplayOrder) ? Ok() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleAvailability(Guid id)
            => await _productService.ToggleProductAvailabilityAsync(id) ? Ok() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _productService.DeleteProductAsync(id) ? Ok() : NotFound();
    }
}