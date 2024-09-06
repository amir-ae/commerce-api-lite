namespace Commerce.Infrastructure.Common.Authentication;

public class JwtSettings
{
    public static string SectionName { get; } = "Authentication:JwtSettings";
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Key { get; init; } = null!;
}