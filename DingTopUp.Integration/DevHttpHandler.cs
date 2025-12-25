namespace DingTopUp.Integration.Auth;

public static class DevHttpHandlers
{
    /// <summary>
    /// DEV / STAGING ONLY – bypasses all TLS certificate checks.
    /// </summary>
    public static HttpClientHandler CreateUnsafeHandler()
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }
}
