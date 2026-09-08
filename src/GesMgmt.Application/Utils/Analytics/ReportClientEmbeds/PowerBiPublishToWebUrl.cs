using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Utils.Analytics;

public static class PowerBiPublishToWebUrl
{
    private const string ExpectedHost = "app.powerbi.com";
    private const string ExpectedPath = "/view";

    public static bool TryNormalize(
        string? value,
        out string normalizedUrl)
    {
        normalizedUrl = string.Empty;

        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Host, ExpectedHost, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.AbsolutePath, ExpectedPath, StringComparison.OrdinalIgnoreCase) ||
            !uri.IsDefaultPort ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment) ||
            !HasExactlyOnePublishCode(uri.Query))
        {
            return false;
        }

        normalizedUrl = uri.AbsoluteUri;
        return true;
    }

    private static bool HasExactlyOnePublishCode(string query)
    {
        var publishCodeCount = 0;

        foreach (var part in query
                     .TrimStart('?')
                     .Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pieces = part.Split('=', 2);

            if (pieces.Length != 2)
            {
                continue;
            }

            string key;

            try
            {
                key = Uri.UnescapeDataString(pieces[0]);
            }
            catch (UriFormatException)
            {
                return false;
            }

            if (!string.Equals(key, "r", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            publishCodeCount++;

            if (publishCodeCount > 1 || string.IsNullOrWhiteSpace(pieces[1]))
            {
                return false;
            }
        }

        return publishCodeCount == 1;
    }
}
