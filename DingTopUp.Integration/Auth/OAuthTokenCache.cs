namespace DingTopUp.Integration.Auth;

public sealed class OAuthTokenCache
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _token;
    private DateTimeOffset _expiresAt;

    public async Task<string> GetOrRefreshAsync(Func<Task<(string token, DateTimeOffset expires)>> refresh)
    {
        if (!string.IsNullOrEmpty(_token) && DateTimeOffset.UtcNow < _expiresAt)
            return _token!;

        await _gate.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(_token) || DateTimeOffset.UtcNow >= _expiresAt)
            {
                var (t, exp) = await refresh();
                _token = t;
                _expiresAt = exp;
            }

            return _token!;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void ExpireNow() => _expiresAt = DateTimeOffset.MinValue;
}
