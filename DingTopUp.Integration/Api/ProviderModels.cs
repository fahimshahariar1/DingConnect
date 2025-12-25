namespace DingTopUp.Integration.Api;

public sealed record GetProvidersResponse(
    int ResultCode,
    List<ApiError> ErrorCodes,
    List<ProviderItem> Items
);

/// <summary>
/// Single provider entry from GetProviders.
/// We only model the fields we actually need right now.
/// </summary>
public sealed class ProviderItem
{
    public string ProviderCode { get; set; } = string.Empty;
    public string CountryIso { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }

    /// <summary>
    /// Regex used to validate / match an account number, e.g. ^88016[0-9]{8}$
    /// </summary>
    public string? ValidationRegex { get; set; }

    public string? CustomerCareNumber { get; set; }

    public List<string> RegionCodes { get; set; } = new();
    public List<string> PaymentTypes { get; set; } = new();

    public string? LogoUrl { get; set; }
}
