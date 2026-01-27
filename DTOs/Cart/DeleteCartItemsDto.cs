namespace FoodDelivery.DTOs.Cart;
public class DeleteCartItemsDto
{
    public List<Guid> ProductIds { get; set; } = new();
}