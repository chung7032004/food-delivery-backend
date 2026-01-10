using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<RefreshToken?> GetRefreshTokenWithUserAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeAllAsync(Guid userId, string? exceptToken = null);
    }
}