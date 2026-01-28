using FoodDelivery.Service.Interfaces;
using FoodDelivery.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers.Admin;
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class UsersManagerController : ControllerBase
{
    private readonly IUserManagerService _adminService;
    public UsersManagerController( IUserManagerService adminService)
    {
        _adminService= adminService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? role = null)
    {
        var result = await _adminService.GetAllUsersAsync(page, pageSize, role);
        return Ok(result);
    }

    // // 2. Xem chi tiết 1 User
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetDetail(Guid id)
    // {
    //     var result = await _adminService.GetUserByIdAsync(id);
    //     if (!result.IsSuccess) return NotFound(result);
    //     return Ok(result);
    // }

    // // 3. Khóa hoặc kích hoạt tài khoản
    // [HttpPatch("{id}/toggle-status")]
    // public async Task<IActionResult> ToggleStatus(Guid id)
    // {
    //     var result = await _adminService.ToggleUserStatusAsync(id);
    //     if (!result.IsSuccess) return BadRequest(result);
    //     return Ok(result);
    // }
}