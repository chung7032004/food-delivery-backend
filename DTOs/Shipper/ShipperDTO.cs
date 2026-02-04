namespace FoodDelivery.DTOs
{
    public class ShipperActionDTO
    {
        public Guid OrderId { get; set; }
        public string? Reason { get; set; } // Chỉ dùng khi giao thất bại
    }

    public class OrderAdminSummaryResponse
    {
        public Guid Id { get; set; }
        public string? OrderCode { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; } // Trả về dạng chuỗi để FE dễ đọc
        public DateTime CreatedAt { get; set; }
    }

    public class OrderDetailResponse
    {
        public Guid OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateShipperProfileDto
    {
        public bool? IsAvailable { get; set; }
    }
    
}