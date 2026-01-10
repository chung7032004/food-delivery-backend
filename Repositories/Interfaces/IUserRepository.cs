using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetUserByIdWithRoleAsync(Guid userId);
        Task<bool> IsEmailExistsAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteSoftAsync(Guid userId);
        Task UpdateLastLoginAsync(Guid userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task ChangePasswordAsync(Guid userId, byte[] passwordHash, byte[] passwordSalt);
       
    }
}