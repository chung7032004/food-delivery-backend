using FoodDelivery.DTOs;

namespace FoodDelivery.Service.Interfaces
{
    public interface IShipperService
    {
        Task<bool> ConfirmPickUpAsync(Guid orderId, Guid userId);
        Task<bool> MarkSuccessAsync(Guid orderId);
        Task<bool> MarkFailedAsync(Guid orderId, string reason, Guid? cancelledBy = null);
    }
}