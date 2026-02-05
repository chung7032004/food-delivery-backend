using FoodDelivery.Entities;
using FoodDelivery.DTOs.User;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetUserByIdWithRoleAsync(Guid userId);
        Task<bool> IsEmailExistsAsync(string email);
        Task<List<User>> GetUsersByRoleAsync(string roleName);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteSoftAsync(Guid userId);
        Task UpdateLastLoginAsync(Guid userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task ChangePasswordAsync(Guid userId, byte[] passwordHash, byte[] passwordSalt);
        Task<List<AdminUserListDto>> GetAllUsersAsync(UserFilterModel filter);
        Task<int> GetTotalUsersCountAsync(UserFilterModel filter);
        Task<AdminUserDetailDto?> GetUserDetailAsync(Guid userId);
        Task<bool> BlockUserAsync(Guid userId);
        Task<bool> UnblockUserAsync(Guid userId);
        Task<List<UserOrderSummary>> GetUserRecentOrdersAsync(Guid userId, int take = 5);
    }
}