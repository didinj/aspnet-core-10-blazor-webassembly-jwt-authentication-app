using System.Net.Http.Headers;
using System.Net;
using Client.Services;
using Shared.Auth;

namespace Client.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly TokenStorage _tokenStorage;
    private readonly AuthService _auth;

    public AuthMessageHandler(TokenStorage tokenStorage, AuthService auth)
    {
        _tokenStorage = tokenStorage;
        _auth = auth;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = _tokenStorage.GetAccessToken();

        // Attach access token to every request if available
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If Unauthorized (token expired), attempt refresh
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Try to refresh token
            var refreshed = await _auth.RefreshAsync();

            if (refreshed)
            {
                // Retry the request with new access token
                accessToken = _tokenStorage.GetAccessToken();

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                // Important: create a NEW request message
                var retryRequest = CloneRequest(request);

                return await base.SendAsync(retryRequest, cancellationToken);
            }
        }

        return response;
    }

    private static HttpRequestMessage CloneRequest(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copy content (if any)
        if (request.Content != null)
            clone.Content = new StreamContent(request.Content.ReadAsStream());

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        foreach (var property in request.Properties)
            clone.Properties.Add(property);

        return clone;
    }
}
