using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class PasswordResetOtpRepository :IPasswordResetOtpRepository
{
    private readonly FoodContext _context;
    public PasswordResetOtpRepository (FoodContext context)
    {
        _context = context;
    }
    public async Task<List<PasswordResetOtp>> GetAllByUserId(Guid userId)
    {
        return await _context.PasswordResetOtp
            .Where(pr=>pr.UserId == userId).ToListAsync();
    }
    public async Task DeleteRangeAsync(IEnumerable<PasswordResetOtp> items)
    {
        _context.PasswordResetOtp.RemoveRange(items);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(PasswordResetOtp passwordResetOtp)
    {
        _context.PasswordResetOtp.Remove(passwordResetOtp);
        await Task.CompletedTask;
    }
    public async Task AddAsync(PasswordResetOtp passwordResetOtp)
    {
        await _context.PasswordResetOtp.AddAsync(passwordResetOtp);
    }
    public async Task<PasswordResetOtp?> GetByEmailAsync(string email)
    {
        return await _context.PasswordResetOtp.Include(pr=>pr.User).FirstOrDefaultAsync(pr=>pr.Email == email);
    }
} 