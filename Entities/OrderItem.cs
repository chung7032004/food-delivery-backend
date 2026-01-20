namespace FoodDelivery.Entities
{
    public class OrderItem
    {
        public Guid Id {get; set;} =  Guid.NewGuid();
        public Guid OrderId {get; set;} 
        public Order Order {get;set;} = null!;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public int Quantity {get; set;} 
        public decimal UnitPrice {get; set;}
        public bool IsRemoved { get; set; } = false;        //món bị bỏ
        public string? RemoveReason { get; set; }  //"Out of stock"
        public Review Review { get; set; } = null!;
    }
}