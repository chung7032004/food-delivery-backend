namespace FoodDelivery.DTOs.Order;
public class CreateOrderResponseDto
{
    public Guid OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee {get; set;}
    public DateTime EstimatedDeliveryTime {get; set;}
}