using FoodDelivery.DTOs.Order;
using FoodDelivery.DTOs.Restaurant;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class RestaurantRepository : IRestaurantRepository
{
    private readonly FoodContext _context;
    public RestaurantRepository (FoodContext context)
    {
        _context = context;
    }
    public async Task<RestaurantProfile?> GetMyRestaurant()
    {
        return await _context.RestaurantProfiles.FirstOrDefaultAsync();   
    }
    public async Task<RestaurantProfile?> GetStatusRestaurantAsync()
    {
        return await _context.RestaurantProfiles
            .AsNoTracking()
            .Select(r => new RestaurantProfile
            {
                IsOpen = r.IsOpen,
                OpenTime = r.OpenTime,
                CloseTime = r.CloseTime,
                ClosingMessage = r.ClosingMessage,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task UpdateAsync (RestaurantProfile restaurant)
    {
        _context.Update(restaurant);
        await Task.CompletedTask;
    }
}