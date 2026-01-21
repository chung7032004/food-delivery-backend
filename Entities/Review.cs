namespace FoodDelivery.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!; // Liên kết tới bảng OrderItem.
        public Guid ProductId { get; set; }
        public Product Product {get; set;} = null!;
        public Guid CustomerId { get; set; }         // Người thực hiện đánh giá
        public User Customer { get; set; } = null!;
        public int Rating { get; set; }          // 1 - 5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
