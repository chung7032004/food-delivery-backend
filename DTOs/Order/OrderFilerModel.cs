namespace FoodDelivery.DTOs.Order;
public class OrderFilterModel
{
    public OrderStatus? Status { get; set; }
    public string? SearchCode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}