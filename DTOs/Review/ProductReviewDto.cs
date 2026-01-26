namespace FoodDelivery.DTOs.Review;
public class ProductReviewDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}