namespace DingTopUp.Integration.Options
{
    public sealed class DingOptions
    {
        public string BaseUrl { get; set; } = default;
        public OAuthOptions OAuth { get; set; } = new();
    }

    public sealed class OAuthOptions
    {
        public string ClientId { get; init; } = default;
        public string ClientSecret { get; init; } = default;
        public string Scope { get; set; } = "topupapi";
        public string TokenEndpoint { get; set; } = default;
    }
}
