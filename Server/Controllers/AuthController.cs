using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Services;
using Shared.Auth;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService) : ControllerBase
{
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = UserStore.Users
            .FirstOrDefault(u => u.Username == request.Username
                              && u.PasswordHash == request.Password);

        if (user == null)
            return Unauthorized("Invalid username or password");

        // Generate access token + expiration
        var accessToken = _tokenService.CreateAccessToken(user, out var accessTokenExpiration);

        // Generate refresh token
        var (refreshToken, refreshExpiry) = _tokenService.CreateRefreshToken();

        // Save refresh token in user store
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = refreshExpiry;

        return Ok(new LoginResponse
        {
            Username = user.Username,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = accessTokenExpiration
        });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenRequest request)
    {
        var user = UserStore.Users.FirstOrDefault(u =>
            u.Username == request.Username &&
            u.RefreshToken == request.RefreshToken &&
            u.RefreshTokenExpiration > DateTime.UtcNow
        );

        if (user == null)
            return Unauthorized("Invalid refresh token");

        // Generate new JWT access token
        var accessToken = _tokenService.CreateAccessToken(user, out var newExpiration);

        // Rotate refresh token (important security measure)
        var (newRefreshToken, newRefreshExpiry) = _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiration = newRefreshExpiry;

        return Ok(new LoginResponse
        {
            Username = user.Username,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Expiration = newExpiration
        });
    }

}
