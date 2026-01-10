using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly FoodContext _context;
        public RefreshTokenRepository(FoodContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt=> rt.Token == token);

        }
        public async Task<RefreshToken?> GetRefreshTokenWithUserAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }
            return await _context.RefreshTokens
                .Include(rt=> rt.User)
                .FirstOrDefaultAsync(rt=> rt.Token == token);
        }
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await Task.CompletedTask;
        }
        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await Task.CompletedTask;
        }
        public async Task RevokeAllAsync(Guid userId, string? exceptToken = null)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt=>rt.UserId == userId && rt.Token != exceptToken && rt.ExpiresAt >DateTime.UtcNow && !rt.IsRevoked)
                .ToListAsync();
            foreach(var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
        }
    }
}