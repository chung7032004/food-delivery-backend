namespace FoodDelivery.Entities
{
    public class Role
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public string Name {get; set;} = string.Empty;
        public virtual ICollection<UserRole> UserRoles {get; set;} = new List<UserRole>();
    }
}