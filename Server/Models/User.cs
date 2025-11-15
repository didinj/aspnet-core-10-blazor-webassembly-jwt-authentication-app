namespace Server.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User";

    // Refresh token support
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}