using Microsoft.Net.Http.Headers;

namespace Commerce.API.Extensions.ETag;

public static class ETagExtensions
{
    public static int? ToVersion(string? header)
    {
        if (string.IsNullOrWhiteSpace(header)) return null;
        if (!EntityTagHeaderValue.TryParse(header, out EntityTagHeaderValue? eTag)) return null;
        var value = eTag.Tag.Value;
        if (string.IsNullOrWhiteSpace(value) || value.Length < 3) return null;
        if (!int.TryParse(value.Substring(1, value.Length - 2), out int result)) return null;
        return result;
    }
}