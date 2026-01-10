using System.Net.Http.Headers;

namespace FoodDelivery.Entities
{
    public class CartItem
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid CartId{get; set;} 
        public virtual Cart Cart {get; set;} = null!;
        public Guid ProductId {get; set;} 
        public Product Product {get; set;} = null!;
        public int Quantity {get; set;}
        public DateTime AddedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
    }
}