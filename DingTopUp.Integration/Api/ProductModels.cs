namespace DingTopUp.Integration.Api;

public sealed record GetProductsResponse(
    int ResultCode,
    List<ApiError> ErrorCodes,
    List<ProductItem> Items
);

/// <summary>
/// Minimal product shape we need for the UI.
/// Extra fields in the JSON will be ignored by System.Text.Json.
/// </summary>
public sealed record ProductItem(
    string ProviderCode,
    string SkuCode,
    string? DefaultDisplayText,
    PriceInfo Minimum,
    PriceInfo Maximum,
    List<string> Benefits,
    bool LookupBillsRequired

);

public sealed record PriceInfo(
    decimal CustomerFee,
    decimal DistributorFee,
    decimal ReceiveValue,
    string ReceiveCurrencyIso,
    decimal ReceiveValueExcludingTax,
    decimal TaxRate,
    string TaxName,
    string TaxCalculation,
    decimal SendValue,
    string SendCurrencyIso
);
