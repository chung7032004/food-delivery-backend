using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email {get; set;} = string.Empty;
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
    public string Password {get; set;} = string.Empty;
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số")]
    public string Phone { get; set; } = string.Empty;
}
public class LoginRequest
{
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
    [MinLength(8)]
    public string Password {get; set;} = string.Empty;
}
public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
public class SendOtpRequest
{
    public string Email { get; set; } =  string.Empty;
}
public class ResetPasswordRequest
{
    public string Email { get; set; } =  string.Empty;
    public string Otp { get; set; } =  string.Empty;
    public string NewPassword { get; set; } =  string.Empty;
}