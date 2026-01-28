using FoodDelivery.Common;
using FoodDelivery.DTOs.Admin.UserManager;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.Service.Interfaces.Admin;

namespace FoodDelivery.Service.Implementations;
public class UserManagerService : IUserManagerService
{
    private readonly IUserRepository _userRepository;
    public UserManagerService (IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Result<PagedResponse<UserAdminSummaryResponse>>>GetAllUsersAsync(int page = 1, int pageSize = 10, string? role = null)
    {
        pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 :pageSize;
        page = page < 1 ? 1:page; 
        var (users, totalCount) = await _userRepository.GetAllUserAsync(page,pageSize,role);
        var meta = new PaginationMeta(
            Page : page,
            PageSize : users.Count(),
            TotalCount : totalCount,
            TotalPages : (int)Math.Ceiling((double)totalCount/pageSize)
        );
        var items = users.Select(u=>new UserAdminSummaryResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            AvatarUrl = u.AvatarUrl,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogin,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
            TotalOrders = u.Orders?.Count ?? 0
        }).ToList();
        var response = new PagedResponse<UserAdminSummaryResponse>(
            Data :items,
            Meta : meta
        );
        return Result<PagedResponse<UserAdminSummaryResponse>>.Success(response);
    }
}