namespace FoodDelivery.DTOs
{
    public class ShipperActionDTO
    {
        public Guid OrderId { get; set; }
        public string? Reason { get; set; } // Chỉ dùng khi giao thất bại
    }
    
}