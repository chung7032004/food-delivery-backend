namespace FoodDelivery.DTOs.Review;
public class ReviewReportDto
{
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> StarCounts { get; set; } = new(); // Key: Số sao (1-5), Value: Số lượng
    public int HiddenReviewsCount { get; set; }
}