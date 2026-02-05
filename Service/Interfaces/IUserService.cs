using FoodDelivery.Common;
using FoodDelivery.DTOs.User;

namespace FoodDelivery.Service.Interfaces;

public interface IUserService
{
    Task<Result<PagedResponse<AdminUserListDto>>> GetAllUsersAsync(UserFilterModel filter);
    Task<Result<AdminUserDetailDto>> GetUserDetailAsync(Guid userId);
    Task<Result> BlockUserAsync(Guid userId);
    Task<Result> UnblockUserAsync(Guid userId);
}
