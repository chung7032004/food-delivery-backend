namespace FoodDelivery.DTOs.Order;
public class OrderAdminSummaryResponse
{
    public Guid Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}