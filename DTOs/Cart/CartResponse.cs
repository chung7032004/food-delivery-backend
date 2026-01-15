namespace FoodDelivery.DTOs.Cart;
public class CartResponse
{
    public Guid CustomerId {get; set;}
    public List<CartItemDto> CartItems = new List<CartItemDto>();
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}