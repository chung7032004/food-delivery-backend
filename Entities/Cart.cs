namespace FoodDelivery.Entities
{
    public class Cart 
    {
        public Guid Id{get; set;} = Guid.NewGuid();
        public Guid CustomerId {get; set;}
        public virtual User? Customer{get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
        public virtual ICollection<CartItem> CartItems {get; set;} = new List<CartItem>();
    }
}