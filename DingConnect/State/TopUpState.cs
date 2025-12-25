using DingTopUp.Integration.Api;

namespace DingConnect.State;

public sealed class TopUpState
{
    public string? CountryIso { get; set; }
    public string? RawPhoneNumber { get; set; }
    public string? NormalizedPhoneNumber { get; set; }

    public ProviderItem? Provider { get; set; }
    public ProductItem? Product { get; set; }

    public void Reset()
    {
        CountryIso = null;
        RawPhoneNumber = null;
        NormalizedPhoneNumber = null;
        Provider = null;
        Product = null;
    }
}
