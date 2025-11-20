namespace DingTopUp.Integration;
public interface IDingApi
{
    Task<GetBalanceResponse> GetBalanceAsync(CancellationToken ct = default);
}
