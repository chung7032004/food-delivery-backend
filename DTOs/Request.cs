using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
    [Required]
    [MinLength(8)]
    public string Password {get; set;} = string.Empty;
    [Required]
    public string FullName { get; set; } = string.Empty;
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