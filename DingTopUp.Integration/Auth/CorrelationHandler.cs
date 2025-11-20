namespace DingTopUp.Integration;

public sealed class CorrelationHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.Headers.TryAddWithoutValidation("X-Correlation-Id", Guid.NewGuid().ToString());
        return base.SendAsync(request, ct);
    }
}
