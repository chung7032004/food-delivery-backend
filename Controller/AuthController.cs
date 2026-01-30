using Microsoft.AspNetCore.Mvc;

using FoodDelivery.Service.Interfaces;
using FoodDelivery.Common;
using FoodDelivery.DTOs;
using Microsoft.AspNetCore.Authorization;
using FoodDelivery.Extensions;

namespace FoodDelivery.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;

    public AuthController(IAuthService authService, IEmailService emailService)
    {
        _emailService = emailService;
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register ([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request.Email, request.Password, request.FullName, request.Phone);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.AuthenticateUserAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "INVALID_INPUT"    => BadRequest(result),
                "USER_NOT_FOUND"   => NotFound(result),
                "INVALID_PASSWORD" => BadRequest(result),
                "USER_INACTIVE"    => StatusCode(StatusCodes.Status403Forbidden, result),
                _                  => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
    {
        if(!User.TryGetUserId(out Guid userId))
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        var result = await _authService.LogoutAsync(userId,logoutRequest.RefreshToken);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken ([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _authService.RefreshAccessTokenAsync(refreshTokenRequest.RefreshToken);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        if(!User.TryGetUserId(out Guid userId))
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        var result = await _authService.ChangePasswordAsync(userId,changePasswordRequest);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        var result = await _authService.SendOtpAsync(request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "EMAIL_INVALID" =>BadRequest(result),
                _ => StatusCode(500,result),
            };
        }
        return Ok(result);
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(
            request.Email, 
            request.Otp, 
            request.NewPassword
        );
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "OTP_LOCKED" => StatusCode(StatusCodes.Status429TooManyRequests, result),
                "OTP_EXPIRED" => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
}