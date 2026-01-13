
namespace FoodDelivery.DTOs;
public class LoginResponse{
    public string? AccessToken {get;set;} = string.Empty;
    public string? RefreshToken {get;set;} = string.Empty;
}
public class RefreshTokenResponse{
    public string? AccessToken {get;set;} = string.Empty;
    public string? RefreshToken {get;set;} = string.Empty;
}
