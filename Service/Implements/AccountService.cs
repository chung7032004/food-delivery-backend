using FoodDelivery.Common;
using FoodDelivery.DTOs.Account;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    public AccountService (IUserRepository userRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }
    public async Task <Result<AccountResponse>> GetAccountAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdWithRoleAsync(userId);
        if(user == null)
        {
            return Result<AccountResponse>.Failure("USER_NOT_FOUND", "User không tồn tại");
        }
        var avatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
            ? "/uploads/avatars/default.png"
            : user.AvatarUrl;
        
        // Get user roles
        var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").Where(r => !string.IsNullOrEmpty(r)).ToList() ?? new List<string>();
        
        return Result<AccountResponse>.Success(new AccountResponse
        {
            AvatarUrl = avatarUrl,
            Email = user.Email ,
            FullName = user.FullName,
            Id = user.Id,
            Phone = user.Phone,
            Roles = roles,
        });
    }
    public async Task<Result> UpdateAccountAsync(Guid userId, AccountRequest accountRequest)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null)
        {
            return Result.Failure("USER_NOT_FOUND", "User không tồn tại");
        }
        if(accountRequest.Avatar != null)
        {
            var uploadResult = await _fileService.SaveFileAsync(accountRequest.Avatar, "avatars");
            if (!uploadResult.IsSuccess || string.IsNullOrEmpty(uploadResult.Data))
            {
                return uploadResult;
            }
            _fileService.DeleteFile(user.AvatarUrl);
            user.AvatarUrl = uploadResult.Data;
        }
        if(accountRequest.FullName != null && accountRequest.FullName.Length > 0) user.FullName = accountRequest.FullName;
        if(accountRequest.Email != null && accountRequest.Email.Length > 0) user.Email = accountRequest.Email;
        if(accountRequest.Phone != null && accountRequest.Phone.Length > 0) user.Phone = accountRequest.Phone;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<AccountResponse>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<AccountResponse>.Failure("USER_NOT_FOUND", "User không tồn tại");
        }

        user.FullName = request.FullName.Trim();
        user.Phone = request.Phone.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<AccountResponse>.Success(new AccountResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
                ? "/uploads/avatars/default.png"
                : user.AvatarUrl,
        });
    }
}