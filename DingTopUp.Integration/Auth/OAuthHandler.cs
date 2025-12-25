using System.Net.Http.Headers;
using System.Net.Http.Json;
using DingTopUp.Integration.Options;
using Microsoft.Extensions.Options;

namespace DingTopUp.Integration.Auth;

public sealed class OAuthHandler : DelegatingHandler
{
    private readonly OAuthTokenCache _cache;
    private readonly OAuthOptions _opt;
    private readonly HttpClient _http;

    public OAuthHandler(IHttpClientFactory factory, IOptions<DingOptions> cfg, OAuthTokenCache cache)
    {
        _cache = cache;
        _opt = cfg.Value.OAuth;
        _http = factory.CreateClient("oauth");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _cache.GetOrRefreshAsync(() => FetchTokenAsync(ct));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await base.SendAsync(request, ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _cache.ExpireNow();
            var token2 = await _cache.GetOrRefreshAsync(() => FetchTokenAsync(ct));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token2);
            return await base.SendAsync(request, ct);
        }

        return resp;
    }

    private async Task<(string token, DateTimeOffset expires)> FetchTokenAsync(CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"] = _opt.ClientId,
            ["client_secret"] = _opt.ClientSecret,
            ["grant_type"] = "client_credentials",
            ["scope"] = _opt.Scope
        };

        var r = await _http.PostAsync(_opt.TokenEndpoint, new FormUrlEncodedContent(form), ct);
        r.EnsureSuccessStatusCode();

        var dto = await r.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
                  ?? throw new InvalidOperationException("Empty token response");

        var expires = DateTimeOffset.UtcNow.AddSeconds(dto.expires_in - 60);
        return (dto.access_token, expires);
    }

    private sealed record TokenResponse(string access_token, int expires_in, string token_type, string scope);
}
