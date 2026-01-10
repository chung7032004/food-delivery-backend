using System.Collections.ObjectModel;

namespace FoodDelivery.Entities
{
        public class User
    {
        public Guid Id { get; set;} = Guid.NewGuid();
        public string FullName {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public byte[] PasswordHash {get; set;} = Array.Empty<byte>();
        public byte[] PasswordSalt {get; set;} = Array.Empty<byte>();
        public bool IsActive {get; set;} = true;
        public DateTime CreatedAt {get; set;}
        public DateTime? UpdatedAt {get; set;}
        public DateTime? LastLogin {get; set;}

        // Navigation
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens {get; set;} = new List<RefreshToken>();
        public virtual ICollection<UserRole> UserRoles {get; set;} = new List<UserRole>();
        public virtual ICollection<Order> Orders {get; set;} = new List<Order>();
        public virtual ICollection<Address> Addresses {get; set;} = new List<Address>();
        public virtual ICollection<Review> Reviews {get; set;} = new List<Review>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories {get; set;} = new List<OrderStatusHistory>();
        

    }
}