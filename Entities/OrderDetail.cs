namespace FoodDelivery.Entities
{
    public class OrderDetail
    {
        public Guid Id {get; set;} =  Guid.NewGuid();
        public Guid OrderId {get; set;} 
        public Order Order {get;set;} = null!;
        public Guid? ShipperId { get; set; }
        public User? Shipper { get; set; }
        public OrderStatus Status {get; set;} 
        public PaymentMethod PaymentMethod {get; set;} 
        public PaymentStatus PaymentStatus {get; set;} 
        public Guid? CancelledByUserId { get; set; }
        public User? CancelledByUser { get; set; }
        public string CancelReason {get; set;} = string.Empty;
        public DateTime? EstimatedDeliveryTime {get; set;}
        public DateTime? ActualDeliveryTime {get; set;}
    }
}