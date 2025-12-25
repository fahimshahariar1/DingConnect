using System.Text.RegularExpressions;

namespace DingConnect.State;

public static class PhoneNormalizer
{
    public static string Normalize(string? input, string? countryIso)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        countryIso ??= "";

        // digits only
        var digits = Regex.Replace(input, "[^0-9]", "");

        // BD normalization: 01XXXXXXXXX => 8801XXXXXXXXX
        if (countryIso.Equals("BD", StringComparison.OrdinalIgnoreCase))
        {
            if (digits.StartsWith("01") && digits.Length == 11)
                return "88" + digits;

            if (digits.StartsWith("880") && digits.Length == 13)
                return digits;
        }

        return digits;
    }
}
