namespace DingTopUp.Integration.Options;

public sealed class DingOptions
{
    public string BaseUrl { get; init; } = default!;
    public OAuthOptions OAuth { get; init; } = new();
}

public sealed class OAuthOptions
{
    public string TokenEndpoint { get; init; } = default!;
    public string ClientId { get; init; } = default!;
    public string ClientSecret { get; init; } = default!;
    public string Scope { get; init; } = "topupapi";
}
