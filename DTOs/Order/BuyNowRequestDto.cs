namespace FoodDelivery.DTOs.Order;
public class BuyNowRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid AddressId { get; set; }
    public string? Note {get; set;} = string.Empty;
}