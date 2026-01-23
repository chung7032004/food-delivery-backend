using FoodDelivery.Common;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Middlewares
{
    public class RestaurantOpenMiddleware
    {
        private readonly RequestDelegate _next;
        public RestaurantOpenMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, FoodContext db)
        {
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method;
            if(path == null || path.StartsWith("/api/admin"))
            {
                await _next(context);
                return;
            }
            var isBuyNow = method == HttpMethods.Post && path == "/api/orders/buy-now";
             var isCheckout =method == HttpMethods.Post &&path == "/api/orders/checkout";
            if(!isBuyNow && !isCheckout)
            {
                await _next(context);
                return;
            } 
            var restaurant = await db.RestaurantProfiles.FirstOrDefaultAsync();
            if(restaurant == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(Result.Failure("RESTAURANT_NOT_CONFIGURED","Cửa hàng chưa được cấu hình."));
                return;
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
            if(finalStatus == false)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(Result.Failure("RESTAURANT_CLOSED",restaurant.ClosingMessage ?? "Quán hiện đang đóng cửa."));
                return;
            }
            await _next(context);
        }
    }
}