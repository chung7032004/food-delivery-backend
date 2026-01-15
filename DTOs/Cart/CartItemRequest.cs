namespace FoodDelivery.DTOs.Cart;
public class CartItemRequest
{
    public Guid ProductId {get; set;} 
    public int Quantity {get; set;}
}