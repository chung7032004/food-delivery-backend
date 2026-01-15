namespace FoodDelivery.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();    
         public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
