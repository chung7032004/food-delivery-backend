namespace FoodDelivery.Entities
{
    public class RefreshToken
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public string Token {get; set;} = string.Empty;
        public DateTime ExpiresAt {get; set;}
        public bool IsRevoked {get; set;} = false;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime? RevokedAt {get; set;}
        public Guid UserId {get; set;}
        public virtual User User {get; set;} = null!;

    }
}