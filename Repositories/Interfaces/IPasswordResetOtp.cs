using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IPasswordResetOtpRepository
    {
        Task<List<PasswordResetOtp>> GetAllByUserId(Guid userId);
        Task DeleteRangeAsync(IEnumerable<PasswordResetOtp> items);
        Task DeleteAsync(PasswordResetOtp passwordResetOtp);
        Task AddAsync(PasswordResetOtp passwordResetOtp);
        Task <PasswordResetOtp?> GetByEmailAsync(string email);
    }
}