namespace FoodDelivery.DTOs.Cart;
public class CartResponse
{
    public Guid CustomerId {get; set;}
    public List<CartItemDto> CartItems {get; set;} = new();
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}