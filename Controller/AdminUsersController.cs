using FoodDelivery.Common;
using FoodDelivery.DTOs.User;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers;

[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/admin/users
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterModel filter)
    {
        var result = await _userService.GetAllUsersAsync(filter);
        return Ok(result);
    }

    // GET api/admin/users/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserDetail(Guid id)
    {
        var result = await _userService.GetUserDetailAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    // PATCH api/admin/users/{id}/block
    [HttpPatch("{id}/block")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BlockUser(Guid id)
    {
        var result = await _userService.BlockUserAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    // PATCH api/admin/users/{id}/unblock
    [HttpPatch("{id}/unblock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnblockUser(Guid id)
    {
        var result = await _userService.UnblockUserAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
