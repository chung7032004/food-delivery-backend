using FoodDelivery.DTOs.Staff;
using FoodDelivery.Entities;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly FoodContext _context;

    public StaffController(IStaffService staffService, FoodContext context)
    {
        _staffService = staffService;
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userId = User.GetUserId();
        var result = await _staffService.GetDashboardStats(userId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrdersForPreparation()
    {
        var userId = User.GetUserId();
        var result = await _staffService.GetOrdersForPreparation(userId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderDetails([FromRoute] Guid orderId)
    {
        var userId = User.GetUserId();
        var result = await _staffService.GetOrderDetails(userId, orderId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("orders/{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        var userId = User.GetUserId();
        var result = await _staffService.UpdateOrderStatus(userId, orderId, request.Status);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _staffService.GetStaffProfile(userId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // Helper endpoint to initialize staff for current user (for testing/setup)
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeStaffProfile()
    {
        var userId = User.GetUserId();
        // Get first restaurant (for testing)
        var restaurant = await _context.RestaurantProfiles.FirstOrDefaultAsync();
        if (restaurant == null)
            return BadRequest(new { message = "No restaurant found" });

        // Check if staff already exists
        var existingStaff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
        if (existingStaff != null)
            return Ok(new { message = "Staff profile already exists", data = existingStaff });

        // Create new staff record
        var staff = new Staff
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RestaurantId = restaurant.Id,
            Position = "Cook",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Staff profile created successfully", data = staff });
    }

    [HttpPost("orders/{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder([FromRoute] Guid orderId)
    {
        var userId = User.GetUserId();
        var result = await _staffService.ConfirmOrder(userId, orderId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("orders/{orderId}/start-preparing")]
    public async Task<IActionResult> StartPreparingOrder([FromRoute] Guid orderId)
    {
        var userId = User.GetUserId();
        var result = await _staffService.StartPreparingOrder(userId, orderId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("orders/{orderId}/mark-ready")]
    public async Task<IActionResult> MarkOrderReady([FromRoute] Guid orderId)
    {
        var userId = User.GetUserId();
        var result = await _staffService.MarkOrderReady(userId, orderId);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("orders/{orderId}/report-issue")]
    public async Task<IActionResult> ReportOrderIssue([FromRoute] Guid orderId, [FromBody] ReportIssueRequest request)
    {
        var userId = User.GetUserId();
        var result = await _staffService.ReportOrderIssue(userId, orderId, request.IssueDescription, request.IssueType);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
