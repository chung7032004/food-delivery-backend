namespace FoodDelivery.DTOs.Product
{
    public class ProductCreateDto
    {
        public Guid CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }
}