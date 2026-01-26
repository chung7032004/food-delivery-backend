namespace FoodDelivery.DTOs.Review;
public class AdminReviewDto
{
    public Guid Id { get; set; }
    public Guid CustomerId {get; set;}
    public string CustomerName { get; set; } = string.Empty;
    public string AvatarUrl {get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsHidden { get; set; } // Admin cần biết cái nào đang bị ẩn
    public DateTime CreatedAt { get; set; }
    public Guid OrderItemId { get; set; } // Để truy vết đơn hàng khi cần
}