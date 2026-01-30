namespace FoodDelivery.DTOs.Notification;

public class NotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Type { get; set; }
    public string? Link { get; set; }
}
