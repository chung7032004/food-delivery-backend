using FoodDelivery.Common;
using FoodDelivery.DTOs.Staff;

namespace FoodDelivery.Service.Interfaces;

public interface IStaffService
{
    Task<Result<StaffStatsDto>> GetDashboardStats(Guid userId);
    Task<Result<List<OrderDto>>> GetOrdersForPreparation(Guid userId);
    Task<Result<OrderDto>> GetOrderDetails(Guid userId, Guid orderId);
    Task<Result<string>> UpdateOrderStatus(Guid userId, Guid orderId, int newStatus);
    Task<Result<StaffProfileDto>> GetStaffProfile(Guid userId);
    Task<Result<string>> ConfirmOrder(Guid userId, Guid orderId);
    Task<Result<string>> StartPreparingOrder(Guid userId, Guid orderId);
    Task<Result<string>> MarkOrderReady(Guid userId, Guid orderId);
    Task<Result<string>> ReportOrderIssue(Guid userId, Guid orderId, string issueDescription, string issueType);
}
