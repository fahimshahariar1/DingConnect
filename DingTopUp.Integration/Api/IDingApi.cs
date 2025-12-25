namespace DingTopUp.Integration.Api;
using DingTopUp.Integration.Models;

public interface IDingApi
{
    Task<GetBalanceResponse> GetBalanceAsync(CancellationToken ct = default);

    /// <summary>
    /// Get providers for a given country.
    /// We’ll use ValidationRegex to auto-detect which provider matches the number.
    /// </summary>
    Task<GetProvidersResponse> GetProvidersAsync(
        string countryIso,
        CancellationToken ct = default);

    /// <summary>
    /// Get products for a provider.
    /// We pass countryIso + accountNumber so Ding can filter & apply the right rules.
    /// </summary>
    Task<GetProductsResponse> GetProductsAsync(
        string providerCode,
        string? countryIso = null,
        string? accountNumber = null,
        CancellationToken ct = default);

    Task<SendTransferResponse> SendTransferAsync(SendTransferRequest request, string? correlationId = null);

}

