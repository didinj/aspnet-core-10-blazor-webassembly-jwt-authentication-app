namespace Shared.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime Expiration { get; set; }
    public string Username { get; set; } = default!;
    public string Role { get; set; } = default!;
}