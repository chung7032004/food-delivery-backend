using FoodDelivery.Common;
using FoodDelivery.DTOs.Notification;

namespace FoodDelivery.Service.Interfaces;

public interface INotificationService
{
    Task<Result<NotificationResponse>> CreateNotificationAsync(Guid userId, NotificationRequest request);
    Task<Result<NotificationSummaryResponse>> GetUserNotificationsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<Result<List<NotificationResponse>>> GetUnreadNotificationsAsync(Guid userId);
    Task<Result<NotificationResponse>> GetNotificationByIdAsync(Guid notificationId);
    Task<Result<int>> GetUnreadCountAsync(Guid userId);
    Task<Result<NotificationResponse>> MarkAsReadAsync(Guid notificationId);
    Task<Result<int>> MarkAllAsReadAsync(Guid userId);
    Task<Result<bool>> DeleteNotificationAsync(Guid notificationId);
}
