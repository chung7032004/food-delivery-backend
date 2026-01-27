namespace FoodDelivery.DTOs.Restaurant;
public class RestaurantProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public string? ClosingMessage { get; set; }
    public TimeSpan OpenTime { get; set; } 
    public TimeSpan CloseTime { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}