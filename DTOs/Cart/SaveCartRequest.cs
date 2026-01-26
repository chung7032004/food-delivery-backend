namespace FoodDelivery.DTOs.Cart;

public class SaveCartRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
}
