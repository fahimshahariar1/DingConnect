using System.Net.Http.Json;

namespace DingTopUp.Integration;

public sealed class DingApi(HttpClient http) : IDingApi
{
    public Task<GetBalanceResponse> GetBalanceAsync(CancellationToken ct = default) =>
        http.GetFromJsonAsync<GetBalanceResponse>("/GetBalance", ct)!;
}
