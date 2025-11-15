using Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Auth;

namespace Client.Services;

public class AuthService
{
    private readonly ApiService _api;
    private readonly TokenStorage _tokens;
    private readonly AppAuthenticationStateProvider _authState;

    public AuthService(ApiService api, TokenStorage tokens, AuthenticationStateProvider authState)
    {
        _api = api;
        _tokens = tokens;
        _authState = (AppAuthenticationStateProvider)authState;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var result = await _api.LoginAsync(new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (result == null)
            return false;

        _tokens.SetAccessToken(result.AccessToken);
        await _tokens.SetRefreshTokenAsync(result.RefreshToken);

        _authState.NotifyUserAuthentication(result.AccessToken);
        return true;
    }

    public async Task<bool> RefreshAsync()
    {
        var refresh = await _tokens.GetRefreshTokenAsync();
        if (refresh == null) return false;

        var result = await _api.RefreshAsync(new RefreshTokenRequest
        {
            Username = "",
            RefreshToken = refresh
        });

        if (result == null) return false;

        _tokens.SetAccessToken(result.AccessToken);
        await _tokens.SetRefreshTokenAsync(result.RefreshToken);

        _authState.NotifyUserAuthentication(result.AccessToken);
        return true;
    }

    public async Task LogoutAsync()
    {
        _tokens.ClearAccessToken();
        await _tokens.ClearRefreshTokenAsync();
        _authState.NotifyUserLogout();
    }
}
