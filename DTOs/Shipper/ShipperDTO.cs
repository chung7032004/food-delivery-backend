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
        public DateTime CreatedAt { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CurrentStatus { get; set; }
        public string? CancelReason { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
        public List<OrderStatusHistoryDTO> StatusHistories { get; set; } = new();
        public Guid? ShipperId { get; set; }
    }

    public class OrderItemDTO
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsRemoved { get; set; }
        public string? RemoveReason { get; set; }
    }

    public class OrderStatusHistoryDTO
    {
        public string? Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid ChangedBy { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateShipperProfileDto
    {
        public bool? IsAvailable { get; set; }
    }
    
}