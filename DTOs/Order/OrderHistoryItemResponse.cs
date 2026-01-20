namespace FoodDelivery.DTOs.Order;
public class OrderHistoryItemResponse
{
    public Guid OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee {get; set;}
    public OrderStatus CurrentStatus { get; set; }
}