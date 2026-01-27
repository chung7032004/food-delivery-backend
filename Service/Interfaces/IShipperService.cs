using FoodDelivery.DTOs;
using FoodDelivery.Entities; // Thêm cái này để nhận diện lớp User, OrderStatusHistory

namespace FoodDelivery.Service.Interfaces
{
    public interface IShipperService
    {
        // --- 3 chức năng nghiệp vụ giao hàng ---
        Task<bool> ConfirmPickUpAsync(Guid orderId, Guid userId);
        Task<bool> MarkSuccessAsync(Guid orderId);
        Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null);

        // --- 5 chức năng quản lý 
        Task<List<User>> GetAllShippersAsync(); // Chức năng 2
        Task<User?> GetShipperByIdAsync(Guid userId); // Chức năng 3
        Task<bool> ToggleShipperStatusAsync(Guid userId, bool isActive); // Chức năng 4
        Task<List<OrderStatusHistory>> GetShipperHistoryAsync(Guid userId); // Chức năng 5
        Task<bool> AssignShipperRoleAsync(Guid userId); // Chức năng 1
    }
}