namespace Shared.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = default!;
    public string Username { get; set; } = default!;
}