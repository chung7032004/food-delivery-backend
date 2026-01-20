namespace FoodDelivery.Entities
{
    public class Address
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Guid UserId {get; set;}
        public virtual User User {get; set;} = null!;
        public string FullAddress {get; set;} = string.Empty;
        public string Label {get; set;} = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault {get; set;} 
        public double Latitude {get; set;}
        public double Longitude {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}