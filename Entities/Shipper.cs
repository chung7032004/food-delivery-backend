namespace FoodDelivery.Entities
{
    public class Shipper
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string? Phone { get; set; }
    }

}