using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodContext _context;
        public UserRepository(FoodContext context)
        {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            return await _context.Users 
                .AsNoTracking()
                .Include(u=>u.UserRoles)
                .ThenInclude(ur=>ur.Role)
                .FirstOrDefaultAsync(u=>u.Email == email);
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<(List<User> users, int totalCount  )> GetAllUserAsync(int page = 1, int pageSize = 10, string? role = null)
        {
            var query = _context.Users.AsNoTracking().Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role).AsQueryable();
            if(!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == role));
            }
            var totalCount =await query.CountAsync();
            var users = await query
                .OrderByDescending(u=>u.CreatedAt)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            return(users,totalCount);
        }
        public async Task<User?> GetUserByIdWithRoleAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u=>u.UserRoles)
                .ThenInclude(ur =>ur.Role)
                .FirstOrDefaultAsync(u=>u.Id == userId);
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return false;
            }
            return await _context.Users.AnyAsync(u=>u.Email == email.Trim());
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await Task.CompletedTask;
        }
        public async  Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;

        }
        public async Task DeleteSoftAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = false; // Đánh dấu là đã xóa
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                user.Email = $"deleted_{timestamp}_{user.Email}";
                user.UpdatedAt = DateTime.UtcNow;
            }
        }
        public async Task UpdateLastLoginAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                _context.Users.Update(user);
            }
        }
        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole); 
        }
        public async Task ChangePasswordAsync(Guid userId, byte[] passwordHash, byte[] passwordSalt)
        {
            var user =await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not Found");
            }
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();
        } 
    }
}