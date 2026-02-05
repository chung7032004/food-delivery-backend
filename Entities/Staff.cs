namespace FoodDelivery.Entities
{
    public class Staff
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid RestaurantId { get; set; }
        
        public User User { get; set; } = null!;
        public RestaurantProfile Restaurant { get; set; } = null!;
        
        public string Position { get; set; } = "Cook"; // Cook, Manager, etc.
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
