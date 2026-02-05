namespace FoodDelivery.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAvatar { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsHidden { get; set; }
}
