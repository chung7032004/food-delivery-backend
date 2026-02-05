using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification);
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
    Task<Notification?> GetByIdAsync(Guid notificationId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Notification> UpdateAsync(Notification notification);
    Task DeleteAsync(Guid notificationId);
    Task<int> MarkAllAsReadAsync(Guid userId);
}
