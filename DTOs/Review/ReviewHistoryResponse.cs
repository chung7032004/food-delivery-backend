namespace FoodDelivery.DTOs.Review
{
    public class ReviewHistoryResponseDto
    {
        public Guid Id { get; set; }
        public Guid productId {get; set;}
        public string ProductName { get; set; } = null!;
        public string? ProductImageUrl { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHidden { get; set; }
    }
}