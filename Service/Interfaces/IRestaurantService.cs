using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.DTOs.Restaurant;

namespace FoodDelivery.Service.Interfaces;
public interface IRestaurantService
{
    Task<Result<RestaurantProfileDto>> GetMyRestaurantAsync();
    Task<Result<RestaurantStatusResponse>> GetStatusRestaurantAsync();
    Task<Result> OpenRestaurantAsync();
    Task<Result> CloseRestaurantAsync(CloseRestaurantRequest request);
    Task<Result> UpdateRestaurantAsync(UpdateRestaurantProfileRequest request);
}
