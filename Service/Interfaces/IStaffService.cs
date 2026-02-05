using FoodDelivery.Common;
using FoodDelivery.DTOs.Staff;
using FoodDelivery.DTOs.Review;

namespace FoodDelivery.Service.Interfaces;

public interface IStaffService
{
    Task<Result<StaffStatsDto>> GetDashboardStats(Guid userId);
    Task<Result<List<OrderDto>>> GetOrdersForPreparation(Guid userId);
    Task<Result<List<OrderDto>>> GetCompletedOrders(Guid userId, int days = 30);
    Task<Result<List<ReviewDto>>> GetRestaurantReviews(Guid userId, int? rating = null);
    Task<Result<OrderDto>> GetOrderDetails(Guid userId, Guid orderId);
    Task<Result<string>> UpdateOrderStatus(Guid userId, Guid orderId, int newStatus);
    Task<Result<StaffProfileDto>> GetStaffProfile(Guid userId);
    Task<Result<string>> ConfirmOrder(Guid userId, Guid orderId);
    Task<Result<string>> StartPreparingOrder(Guid userId, Guid orderId);
    Task<Result<string>> MarkOrderReady(Guid userId, Guid orderId);
    Task<Result<string>> ReportOrderIssue(Guid userId, Guid orderId, string issueDescription, string issueType);
}
