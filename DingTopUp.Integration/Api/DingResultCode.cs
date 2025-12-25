namespace DingTopUp.Integration.Api;

public static class DingResultCode
{
    // Based on your real API response: ResultCode = 1 with data = success.
    public const int Success = 1;

    public static bool IsSuccess(int resultCode) =>
        resultCode == Success || resultCode == 0; // keep tolerant for other endpoints/envs
}
