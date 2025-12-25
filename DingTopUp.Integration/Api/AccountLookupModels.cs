namespace DingTopUp.Integration.Api;

public sealed record AccountLookupResponse(
    string CountryIso,
    string AccountNumberNormalized,
    List<AccountLookupItem> Items,
    int ResultCode,
    List<ApiError> ErrorCodes
);

public sealed record AccountLookupItem(
    string ProviderCode,
    string RegionCode
);

