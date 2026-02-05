namespace FoodDelivery.DTOs.Notification;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Link { get; set; }
}

public class NotificationSummaryResponse
{
    public int UnreadCount { get; set; }
    public List<NotificationResponse> Notifications { get; set; } = new List<NotificationResponse>();
}
