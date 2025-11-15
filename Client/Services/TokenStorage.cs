using Microsoft.JSInterop;

namespace Client.Services;

public class TokenStorage
{
    private readonly IJSRuntime _js;
    private string? _accessToken;

    public TokenStorage(IJSRuntime js)
    {
        _js = js;
    }

    // Access token (in-memory only)
    public string? GetAccessToken() => _accessToken;
    public void SetAccessToken(string token) => _accessToken = token;
    public void ClearAccessToken() => _accessToken = null;

    // Refresh token (localStorage)
    public async Task<string?> GetRefreshTokenAsync()
        => await _js.InvokeAsync<string>("localStorage.getItem", "refreshToken");

    public async Task SetRefreshTokenAsync(string token)
        => await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", token);

    public async Task ClearRefreshTokenAsync()
        => await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
}
