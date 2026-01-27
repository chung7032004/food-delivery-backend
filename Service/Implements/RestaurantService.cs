using FoodDelivery.Common;
using FoodDelivery.DTOs.Restaurant;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class RestaurantService :IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly FoodContext _context;
    public RestaurantService(IRestaurantRepository restaurantRepository,FoodContext context)
    {
        _restaurantRepository = restaurantRepository;
        _context = context;
    }
    public async Task<Result<RestaurantProfileDto>> GetMyRestaurantAsync()
    {
        var restaurant = await _restaurantRepository.GetMyRestaurant();
        if(restaurant == null)
        {
            return Result<RestaurantProfileDto>.Failure("RESTAURANT_NOT_FOUND","Thông tin nhà hàng chưa được thiết lập");
        }
        var now = DateTime.Now.TimeOfDay;
        bool isOperating ;
        if(restaurant.OpenTime <= restaurant.CloseTime)
        {
            isOperating = now >= restaurant.OpenTime && now <= restaurant.CloseTime;
        }
        else
        {
            isOperating = now >= restaurant.OpenTime || now <= restaurant.CloseTime;
        }
        var finalStatus = restaurant.IsOpen && isOperating;
        var response = new RestaurantProfileDto
        {
            Name = restaurant.Name,
            Address = restaurant.Address,
            Phone = restaurant.Phone,
            OpenTime = restaurant.OpenTime,
            CloseTime = restaurant.CloseTime,
            IsOpen = finalStatus, // Trạng thái thực tế
            ClosingMessage = restaurant.ClosingMessage,
            Latitude = restaurant.Latitude,
            Longitude = restaurant.Longitude
        };
        return Result<RestaurantProfileDto>.Success(response); 
    }
    public async Task<Result<RestaurantStatusResponse>> GetStatusRestaurantAsync()
    {
        var restaurant = await _restaurantRepository.GetStatusRestaurantAsync();
        if(restaurant == null)
        {
            return Result<RestaurantStatusResponse>.Failure("RESTAURANT_NOT_FOUND","Thông tin nhà hàng chưa được thiết lập");
        }
        var now = DateTime.Now.TimeOfDay;
        bool isOperating ;
        if(restaurant.OpenTime <= restaurant.CloseTime)
        {
            isOperating = now >= restaurant.OpenTime && now <= restaurant.CloseTime;
        }
        else
        {
            isOperating = now >= restaurant.OpenTime || now <= restaurant.CloseTime;
        }
        var finalStatus = restaurant.IsOpen && isOperating;
        var response = new RestaurantStatusResponse
        {
            IsOpen = finalStatus,
            ClosingMessage = restaurant.ClosingMessage,
            UpdatedAt = restaurant.UpdatedAt,
        };
        return Result<RestaurantStatusResponse>.Success(response);
    }
    public async Task<Result> OpenRestaurantAsync()
    {
        var restaurant = await _restaurantRepository.GetMyRestaurant();
        if(restaurant == null)
        {
            return Result.Failure("RESTAURANT_NOT_FOUND","Thông tin nhà hàng chưa được thiết lập");
        }
        if (restaurant.IsOpen)
        {
            return Result.Failure("RESTAURANT_ALREADY_OPEN","Quán đang mở");
        }
        restaurant.IsOpen = true;
        restaurant.ClosingMessage = null;
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _restaurantRepository.UpdateAsync(restaurant);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> CloseRestaurantAsync(CloseRestaurantRequest request)
    {
        var restaurant = await _restaurantRepository.GetMyRestaurant();
        if(restaurant == null)
        {
            return Result.Failure("RESTAURANT_NOT_FOUND","Thông tin nhà hàng chưa được thiết lập");
        }
        if (!restaurant.IsOpen)
        {
            return Result.Failure("RESTAURANT_ALREADY_CLOSE","Quán đang đóng");
        }
        restaurant.IsOpen = false;
        restaurant.ClosingMessage = request.ClosingMessage;
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _restaurantRepository.UpdateAsync(restaurant);
        await _context.SaveChangesAsync();
        return Result.Success();    
    }
    public async Task<Result> UpdateRestaurantAsync(UpdateRestaurantProfileRequest request)
    {
        var restaurant = await _restaurantRepository.GetMyRestaurant();
        if(restaurant == null)
        {
            return Result.Failure("RESTAURANT_NOT_FOUND","Thông tin nhà hàng chưa được thiết lập");
        }
        restaurant.Name = request.Name;
        restaurant.Address = request.Address;
        restaurant.Phone = request.Phone;
        restaurant.Latitude = request.Latitude;
        restaurant.Longitude = request.Longitude;
        restaurant.OpenTime = request.OpenTime;
        restaurant.CloseTime = request.CloseTime;
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _restaurantRepository.UpdateAsync(restaurant);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
