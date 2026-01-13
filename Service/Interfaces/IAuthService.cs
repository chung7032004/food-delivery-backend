using FoodDelivery.Common;
using FoodDelivery.DTOs;

namespace FoodDelivery.Service.Interfaces
{
    public interface IAuthService
    {
        Task <Result> RegisterUserAsync(string email, string password, string fullName);
        Task <Result<LoginResponse>> AuthenticateUserAsync(string email, string password);
        Task <Result<RefreshTokenResponse>> RefreshAccessTokenAsync(string refreshToken);
        Task <Result> LogoutAsync (Guid userId,string refreshToken);
        Task <Result> ChangePasswordAsync (Guid userId,ChangePasswordRequest changePasswordRequest);
    }
}