namespace FoodDelivery.DTOs.Restaurant
{
    public class RestaurantStatusResponse
    {
        public bool IsOpen { get; set; }
        public string? ClosingMessage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}