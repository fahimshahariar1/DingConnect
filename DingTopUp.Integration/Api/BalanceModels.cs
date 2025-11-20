namespace DingTopUp.Integration;

public sealed record GetBalanceResponse(decimal Balance, string CurrencyIso, int ResultCode, List<ApiError> ErrorCodes);
public sealed record ApiError(string Code, string? Context);
