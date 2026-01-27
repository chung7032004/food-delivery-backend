using FoodDelivery.DTOs.Restaurant;
using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface IRestaurantRepository
{
    Task<RestaurantProfile?> GetMyRestaurant();
    Task<RestaurantProfile?> GetStatusRestaurantAsync();
    Task UpdateAsync (RestaurantProfile restaurant);
}