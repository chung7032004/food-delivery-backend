namespace FoodDelivery.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; } // ORDER, PAYMENT, PROMOTION, SYSTEM, REVIEW
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Link { get; set; } // Link to related resource (e.g., /orders/123)
    
    // Foreign key
    public User User { get; set; }
}

public enum NotificationType
{
    ORDER = 1,
    PAYMENT = 2,
    PROMOTION = 3,
    SYSTEM = 4,
    REVIEW = 5,
    DELIVERY = 6
}
