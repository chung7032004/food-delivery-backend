namespace FoodDelivery.DTOs.Order;

public class ShippingFeeResponseDto
{
    public decimal ShippingFee { get; set; }
    public double Distance { get; set; }
    public int EstimatedMinutes { get; set; }
}
