using FoodDelivery.DTOs.Notification;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetUserNotificationsAsync(userId, pageNumber, pageSize);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetUnreadNotificationsAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetUnreadCountAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotificationById([FromRoute] Guid notificationId)
    {
        var result = await _notificationService.GetNotificationByIdAsync(notificationId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { message = "Title and Message are required" });

        // Nếu không có userId trong request, lấy từ token (gửi cho chính mình)
        var userId = User.GetUserId();
        var result = await _notificationService.CreateNotificationAsync(userId, request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetNotificationById), new { notificationId = result.Data?.Id }, result);
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid notificationId)
    {
        var result = await _notificationService.MarkAsReadAsync(notificationId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.MarkAllAsReadAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] Guid notificationId)
    {
        var result = await _notificationService.DeleteNotificationAsync(notificationId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
