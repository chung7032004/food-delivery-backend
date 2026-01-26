namespace FoodDelivery.DTOs.Cart;
public class CartResponse
{
    public Guid CustomerId {get; set;}
    public List<CartItemDto> CartItems {get; set;} = new();
    
    // Alias for frontend compatibility
    public List<CartItemDto> Items 
    { 
        get => CartItems; 
        set => CartItems = value; 
    }
    
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    
    public decimal Total 
    { 
        get => CartItems.Sum(x => x.Price * x.Quantity); 
    }
}