namespace FoodDelivery.Entities;
public class RestaurantProfile
{
    public Guid Id { get; set; }  = Guid.NewGuid() ;
    public string Name { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public string? ClosingMessage { get; set; }
    public DateTime UpdatedAt { get; set; }
}