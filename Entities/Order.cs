namespace FoodDelivery.Entities
{
    public class Order
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid CustomerId {get; set;} 
        public User Customer { get; set; } = null!;
        public string OrderCode { get; set; } = string.Empty;
        public string ShippingAddress {get; set;} = string.Empty;
        public string ReceiverName {get; set;} = string.Empty;
        public string ReceiverPhone {get; set;} = string.Empty;
        public decimal TotalAmount {get; set;}
        public decimal ShippingFee {get; set;}
        public string? Note {get; set;}
        public DateTime CreatedAt{get; set;}
    
        public virtual OrderDetail OrderDetail {get; set;} = null!; 
        public virtual ICollection<OrderItem> OrderItems {get; set;} = new List<OrderItem>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories {get; set;} = new List<OrderStatusHistory>();
        

    }
}