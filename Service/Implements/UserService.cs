using FoodDelivery.Common;
using FoodDelivery.DTOs.User;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedResponse<AdminUserListDto>>> GetAllUsersAsync(UserFilterModel filter)
    {
        try
        {
            var users = await _userRepository.GetAllUsersAsync(filter);
            var totalCount = await _userRepository.GetTotalUsersCountAsync(filter);

            var meta = new PaginationMeta(filter.Page, filter.PageSize, totalCount, (totalCount + filter.PageSize - 1) / filter.PageSize);
            var response = new PagedResponse<AdminUserListDto>(users, meta);

            return Result<PagedResponse<AdminUserListDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PagedResponse<AdminUserListDto>>.Failure("ERROR", $"Lỗi lấy danh sách người dùng: {ex.Message}");
        }
    }

    public async Task<Result<AdminUserDetailDto>> GetUserDetailAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetUserDetailAsync(userId);
            if (user == null)
            {
                return Result<AdminUserDetailDto>.Failure("USER_NOT_FOUND", "Người dùng không tồn tại");
            }

            return Result<AdminUserDetailDto>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<AdminUserDetailDto>.Failure("ERROR", $"Lỗi lấy thông tin người dùng: {ex.Message}");
        }
    }

    public async Task<Result> BlockUserAsync(Guid userId)
    {
        try
        {
            var result = await _userRepository.BlockUserAsync(userId);
            if (!result)
            {
                return Result.Failure("USER_NOT_FOUND", "Người dùng không tồn tại");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("ERROR", $"Lỗi khóa tài khoản: {ex.Message}");
        }
    }

    public async Task<Result> UnblockUserAsync(Guid userId)
    {
        try
        {
            var result = await _userRepository.UnblockUserAsync(userId);
            if (!result)
            {
                return Result.Failure("USER_NOT_FOUND", "Người dùng không tồn tại");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("ERROR", $"Lỗi mở khóa tài khoản: {ex.Message}");
        }
    }
}
