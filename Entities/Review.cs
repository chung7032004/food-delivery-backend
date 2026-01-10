namespace FoodDelivery.Entities
{
    public class Review
    {
        public Guid Id {get; set;} =  Guid.NewGuid();
        public Guid OrderId {get; set;} 
        public Order Order {get;set;} = null!;
        public Guid CustomerId {get; set;}
        public User Customer {get; set;} = null!;
        public int Rating {get; set;} 
        public string Comment {get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;}
    }
}