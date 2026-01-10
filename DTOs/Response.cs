public class LoginResponse{
    public string? AccessToken {get;set;} = string.Empty;
    public string? RefreshToken {get;set;} = string.Empty;
}
public class Response
{
    public bool Success {get;set;} = false;
    public string Message {get;set;} = string.Empty;
}
public class RefreshTokenResponse{
    public string? AccessToken {get;set;} = string.Empty;
    public string? RefreshToken {get;set;} = string.Empty;
}
