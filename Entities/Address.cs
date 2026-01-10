namespace FoodDelivery.Entities
{
    public class Address
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid UserId {get; set;}
        public virtual User User {get; set;} = null!;
        public string FullAddress {get; set;} = string.Empty;
        public string Label {get; set;} = string.Empty;
        public bool IsDefault {get; set;} 
        public decimal Latitude {get; set;}
        public decimal Longitude {get; set;}
        public virtual ICollection<Order> Orders {get; set;} = new List<Order>();
        
    }
}