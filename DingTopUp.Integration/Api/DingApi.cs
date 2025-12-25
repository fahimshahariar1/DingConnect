using System.Net.Http.Headers;
using System.Text.Json;
using DingTopUp.Integration.Models;
namespace DingTopUp.Integration.Api;

public sealed class DingApi : IDingApi
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;

    public DingApi(HttpClient http)
    {
        _http = http;

        // Ensure we always request JSON (Postman does this implicitly)
        _http.DefaultRequestHeaders.Accept.Clear();
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(CancellationToken ct = default)
        => await GetJsonOrThrow<GetBalanceResponse>("GetBalance", ct);

    public async Task<GetProvidersResponse> GetProvidersAsync(string countryIso, CancellationToken ct = default)
    {
        // IMPORTANT: CALL EXACTLY LIKE POSTMAN
        // No query string (we filter by country in app)
        var response = await GetJsonOrThrow<GetProvidersResponse>("GetProviders", ct);

        // Filter here (safe, matches your requirement)
        if (!string.IsNullOrWhiteSpace(countryIso) && response.Items is not null)
        {
            response = response with
            {
                Items = response.Items
                    .Where(p => string.Equals(p.CountryIso, countryIso, StringComparison.OrdinalIgnoreCase))
                    .ToList()
            };
        }

        return response;
    }

    public async Task<GetProductsResponse> GetProductsAsync(
        string providerCode,
        string? countryIso = null,
        string? accountNumber = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(providerCode))
            throw new ArgumentException("Provider code is required.", nameof(providerCode));

        // EXACTLY LIKE POSTMAN: GetProducts?providerCodes=AXBD
        // Add optional params only if you want, but providerCodes is mandatory.
        var parts = new List<string>
        {
            $"providerCodes={Uri.EscapeDataString(providerCode)}"
        };

        // Some Ding environments accept these; keeping optional:
        if (!string.IsNullOrWhiteSpace(countryIso))
            parts.Add($"countryIsos={Uri.EscapeDataString(countryIso)}");

        if (!string.IsNullOrWhiteSpace(accountNumber))
            parts.Add($"accountNumber={Uri.EscapeDataString(accountNumber)}");

        var url = "GetProducts?" + string.Join("&", parts);

        return await GetJsonOrThrow<GetProductsResponse>(url, ct);
    }

    private async Task<T> GetJsonOrThrow<T>(string relativeUrl, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

        using var res = await _http.SendAsync(req, ct);

        var body = await res.Content.ReadAsStringAsync(ct);

        // If HTTP failed (401/403/500) show real details
        if (!res.IsSuccessStatusCode)
        {
            throw new DingHttpException(
                $"HTTP {(int)res.StatusCode} {res.ReasonPhrase} calling '{relativeUrl}'.",
                relativeUrl,
                (int)res.StatusCode,
                body);
        }

        // Deserialize
        T? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<T>(body, JsonOptions);
        }
        catch (Exception ex)
        {
            throw new DingHttpException(
                $"Failed to parse JSON from '{relativeUrl}'. Error: {ex.Message}",
                relativeUrl,
                (int)res.StatusCode,
                body);
        }

        if (parsed is null)
        {
            throw new DingHttpException(
                $"Empty JSON response from '{relativeUrl}'.",
                relativeUrl,
                (int)res.StatusCode,
                body);
        }

        return parsed;
    }

    public async Task<SendTransferResponse> SendTransferAsync(SendTransferRequest request, string? correlationId = null)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post, ("SendTransfer"));

        msg.Headers.Accept.Clear();
        msg.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(correlationId))
            msg.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);

        msg.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(request, JsonOptions),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        using var resp = await _http.SendAsync(msg);

        var raw = await resp.Content.ReadAsStringAsync();
        var contentType = resp.Content.Headers.ContentType?.MediaType ?? "";

        // 1) Handle non-JSON (like 404)
        if (!contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            var preview = raw.Length > 400 ? raw[..400] + "..." : raw;
            throw new InvalidOperationException(
                $"SendTransfer returned non-JSON response. " +
                $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. " +
                $"ResolvedUrl='{msg.RequestUri}'. Content-Type='{contentType}'. Body preview: {preview}"
            );
        }

        // 2) Parse JSON even if ResultCode indicates failure
        var body = System.Text.Json.JsonSerializer.Deserialize<SendTransferResponse>(raw, JsonOptions)
                   ?? throw new InvalidOperationException("SendTransfer returned empty JSON.");

        // Ding often returns HTTP 200 with ResultCode != 1 (like InsufficientBalance)
        return body;
    }


}



/// <summary>
/// Gives you the exact status + response body, so debugging is one-pass.
/// </summary>
public sealed class DingHttpException : Exception
{
    public string Url { get; }
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public DingHttpException(string message, string url, int statusCode, string responseBody)
        : base(message)
    {
        Url = url;
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
