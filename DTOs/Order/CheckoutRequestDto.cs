namespace FoodDelivery.DTOs.Order;
public class CheckoutRequestDto
{
    public Guid AddressId { get; set; }
    public List<Guid> CartItemIds { get; set; } = new List<Guid>();
    public string? Note {get; set;} = string.Empty;
}