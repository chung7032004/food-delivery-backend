namespace FoodDelivery.DTOs.Notification;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Link { get; set; }
}

public class NotificationSummaryResponse
{
    public int UnreadCount { get; set; }
    public List<NotificationResponse> Notifications { get; set; }
}
