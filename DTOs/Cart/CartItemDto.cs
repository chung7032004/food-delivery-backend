namespace FoodDelivery.DTOs.Cart;
public class CartItemDto
{
    public Guid Id {get; set;}
    public Guid ProductId {get; set;} 
    public int Quantity {get; set;}
    public Guid CategoryId { get; set; }
    public string CategoryName {get; set;} = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime AddedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}