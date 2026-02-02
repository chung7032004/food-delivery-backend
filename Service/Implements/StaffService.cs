using FoodDelivery.Common;
using FoodDelivery.DTOs.Staff;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implements;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    public async Task<Result<StaffStatsDto>> GetDashboardStats(Guid userId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<StaffStatsDto>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var todayOrders = await _staffRepository.GetOrdersByRestaurant(staff.RestaurantId);
        var todaysOrders = todayOrders.Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date).ToList();
        
        var stats = new StaffStatsDto
        {
            TotalOrdersToday = todaysOrders.Count,
            PreparingCount = todaysOrders.Count(o => o.Status == 4),
            ReadyCount = todaysOrders.Count(o => o.Status == 5),
            IssueCount = 0
        };

        return Result<StaffStatsDto>.Success(stats);
    }

    public async Task<Result<List<OrderDto>>> GetOrdersForPreparation(Guid userId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<List<OrderDto>>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var orders = await _staffRepository.GetPendingOrdersByRestaurant(staff.RestaurantId);
        return Result<List<OrderDto>>.Success(orders);
    }

    public async Task<Result<OrderDto>> GetOrderDetails(Guid userId, Guid orderId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<OrderDto>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var order = await _staffRepository.GetOrderDetails(orderId);
        if (order == null)
            return Result<OrderDto>.Failure("ORDER_NOT_FOUND", "Order not found");

        return Result<OrderDto>.Success(order);
    }

    public async Task<Result<string>> UpdateOrderStatus(Guid userId, Guid orderId, int newStatus)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<string>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var success = await _staffRepository.UpdateOrderStatusAsync(orderId, newStatus);
        if (!success)
            return Result<string>.Failure("UPDATE_FAILED", "Failed to update order status");

        return Result<string>.Success("Order status updated successfully");
    }

    public async Task<Result<StaffProfileDto>> GetStaffProfile(Guid userId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<StaffProfileDto>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var profileDto = new StaffProfileDto
        {
            Id = staff.Id,
            UserId = staff.UserId,
            RestaurantId = staff.RestaurantId,
            Position = staff.Position,
            IsActive = staff.IsActive,
            RestaurantName = staff.Restaurant?.Name ?? "Unknown",
            UserName = staff.User?.FullName ?? "Unknown"
        };

        return Result<StaffProfileDto>.Success(profileDto);
    }

    public async Task<Result<string>> ConfirmOrder(Guid userId, Guid orderId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<string>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        // Status 3 = Confirmed
        var success = await _staffRepository.UpdateOrderStatusAsync(orderId, 3);
        if (!success)
            return Result<string>.Failure("UPDATE_FAILED", "Failed to confirm order");

        return Result<string>.Success("Order confirmed successfully");
    }

    public async Task<Result<string>> StartPreparingOrder(Guid userId, Guid orderId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<string>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        // Status 4 = Preparing
        var success = await _staffRepository.UpdateOrderStatusAsync(orderId, 4);
        if (!success)
            return Result<string>.Failure("UPDATE_FAILED", "Failed to start preparing order");

        return Result<string>.Success("Order preparation started");
    }

    public async Task<Result<string>> MarkOrderReady(Guid userId, Guid orderId)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<string>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        // Status 5 = ReadyForPickup
        var success = await _staffRepository.UpdateOrderStatusAsync(orderId, 5);
        if (!success)
            return Result<string>.Failure("UPDATE_FAILED", "Failed to mark order as ready");

        return Result<string>.Success("Order marked as ready for pickup");
    }

    public async Task<Result<string>> ReportOrderIssue(Guid userId, Guid orderId, string issueDescription, string issueType)
    {
        var staff = await _staffRepository.GetStaffByUserId(userId);
        if (staff == null)
            return Result<string>.Failure("STAFF_NOT_FOUND", "Staff profile not found");

        var success = await _staffRepository.ReportOrderIssueAsync(orderId, issueDescription, issueType);
        if (!success)
            return Result<string>.Failure("REPORT_FAILED", "Failed to report issue");

        return Result<string>.Success("Issue reported to admin");
    }
}
