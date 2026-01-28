using FoodDelivery.Common;
using FoodDelivery.DTOs.Admin.UserManager;

namespace FoodDelivery.Service.Interfaces.Admin;
public interface IUserManagerService
{
    Task<Result<PagedResponse<UserAdminSummaryResponse>>>GetAllUsersAsync(int page = 1, int pageSize = 10, string? role = null);
}