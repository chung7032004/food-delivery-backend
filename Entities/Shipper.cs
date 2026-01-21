namespace FoodDelivery.Entities
{
    public class Shipper
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsActive { get; set; } = true;// Khóa tài khoảng shipper
        public bool IsAvailable { get; set; } = true;// có đơn hay không
        public int TotalDeliveredOrders { get; set; } = 0;// Tổng số đơn hàng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<OrderDetail> Orders { get; set; } = new List<OrderDetail>();
    }

}