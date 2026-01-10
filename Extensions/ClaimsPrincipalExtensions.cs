using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodDelivery.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
        {
            var claim = user.FindFirst(JwtRegisteredClaimNames.Sub)
                      ?? user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null && Guid.TryParse(claim.Value, out userId))
            {
                return true;
            }

            userId = Guid.Empty;
            return false;
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (TryGetUserId(user, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Không tìm thấy UserId trong token.");
        }
    }
}