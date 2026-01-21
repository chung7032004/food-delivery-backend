using System.ComponentModel.DataAnnotations;

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
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Rating { get; set; }          // 1 - 5
        [MaxLength(1000)]
        public string? Comment { get; set; }
        public bool IsHidden { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
