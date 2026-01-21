using FoodDelivery.Common;
using FoodDelivery.DTOs.Account;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;
    public AccountController ( IAccountService accountService)
    {
        _service = accountService;
    }
    [HttpGet()]
    public async Task<IActionResult> GetAccount()
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result<AccountResponse>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _service.GetAccountAsync(userId);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPut()]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAccount([FromForm] AccountRequest accountRequest)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result<AccountResponse>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _service.UpdateAccountAsync(userId, accountRequest);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
