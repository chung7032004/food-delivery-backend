namespace FoodDelivery.Entities
{
    public class Product
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid CategoryId {get; set;}
        public virtual Category Category {get; set;} = null!;
        public string Name {get; set;} = string.Empty;
        public decimal Price {get; set;} 
        public string ImageUrl {get; set;} = string.Empty;
        public bool IsAvailable {get; set;}  
        public bool IsFeature {get; set;}  
        public int DisplayOrder {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime? UpdateAt {get; set;}
        public virtual ICollection<CartItem> CartItems {get; set;} = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems {get; set;} = new List<OrderItem>();
        
    }
}