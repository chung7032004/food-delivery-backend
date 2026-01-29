namespace FoodDelivery.DTOs.Notification;

public class NotificationRequest
{
    public string Title { get; set; }
    public string Message { get; set; }
    public int Type { get; set; }
    public string? Link { get; set; }
}
