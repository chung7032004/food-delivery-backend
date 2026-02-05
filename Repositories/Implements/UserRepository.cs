using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.DTOs.User;
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
        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name.ToLower() == roleName.ToLower()))
                .ToListAsync();
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
                await _context.SaveChangesAsync();
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

        public async Task<List<AdminUserListDto>> GetAllUsersAsync(UserFilterModel filter)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.Orders)
                .AsQueryable();

            // Filter by search query
            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                var searchTerm = filter.SearchQuery.ToLower();
                query = query.Where(u => 
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Phone.Contains(searchTerm)
                );
            }

            // Filter by active status
            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            var users = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    TotalOrders = u.Orders.Count,
                    TotalSpent = u.Orders.Where(o => o.OrderDetail.Status == OrderStatus.Completed).Sum(o => o.TotalAmount),
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<int> GetTotalUsersCountAsync(UserFilterModel filter)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                var searchTerm = filter.SearchQuery.ToLower();
                query = query.Where(u => 
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Phone.Contains(searchTerm)
                );
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            return await query.CountAsync();
        }

        public async Task<AdminUserDetailDto?> GetUserDetailAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderDetail)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var recentOrders = await GetUserRecentOrdersAsync(userId, 5);

            return new AdminUserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLogin = user.LastLogin,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                TotalOrders = user.Orders.Count,
                TotalSpent = user.Orders.Where(o => o.OrderDetail != null && o.OrderDetail.Status == OrderStatus.Completed).Sum(o => o.TotalAmount),
                RecentOrders = recentOrders
            };
        }

        public async Task<bool> BlockUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserOrderSummary>> GetUserRecentOrdersAsync(Guid userId, int take = 5)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderDetail)
                .OrderByDescending(o => o.CreatedAt)
                .Take(take)
                .Select(o => new UserOrderSummary
                {
                    OrderId = o.Id,
                    OrderCode = o.OrderCode,
                    Amount = o.TotalAmount,
                    Status = o.OrderDetail != null ? (int)o.OrderDetail.Status : 0,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }
    }
}