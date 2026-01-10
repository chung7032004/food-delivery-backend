namespace FoodDelivery.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid OrderId {get; set;}
        public Order Order {get; set;} = null!;
        public OrderStatus Status {get; set;}
        public Guid ChangeByUserId {get; set;}
        public User User {get; set;} = null!;
        public DateTime ChangedAt {get; set;}
        public string Note {get;set;} = string.Empty;
    }
}