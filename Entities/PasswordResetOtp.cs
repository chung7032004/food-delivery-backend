namespace FoodDelivery.Entities
{
    public class PasswordResetOtp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public byte[] OtpHash { get; set; } = Array.Empty<byte>();
        public byte[] OtpSalt {get; set;} = Array.Empty<byte>();
        public DateTime ExpiresAt { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
