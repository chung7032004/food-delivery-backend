using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implements;

public class NotificationRepository : INotificationRepository
{
    private readonly FoodContext _context;

    public NotificationRepository(FoodContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        return notification;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
    {
        var skip = (pageNumber - 1) * pageSize;
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification> GetByIdAsync(Guid notificationId)
    {
        return await _context.Notifications.FindAsync(notificationId);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        return notification;
    }

    public async Task DeleteAsync(Guid notificationId)
    {
        var notification = await GetByIdAsync(notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
        }
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        return notifications.Count;
    }
}
