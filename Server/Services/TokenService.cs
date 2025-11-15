using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server.Models;

namespace Server.Services;

public class TokenService(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public string CreateAccessToken(User user, out DateTime expiration)
    {
        var jwtSettings = _config.GetSection("Jwt");

        expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(jwtSettings["AccessTokenValidityMinutes"]!)
        );

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string RefreshToken, DateTime Expiration) CreateRefreshToken()
    {
        var expiration = DateTime.UtcNow.AddDays(
            double.Parse(_config["Jwt:RefreshTokenValidityDays"]!)
        );

        return (Guid.NewGuid().ToString(), expiration);
    }
}
