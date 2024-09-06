using System.Diagnostics.CodeAnalysis;

namespace Commerce.API.Contract.V1.Common.Requests;

public record PhoneNumber
{
    public PhoneNumber()
    {
    }
    
    [SetsRequiredMembers]
    public PhoneNumber(
        string value,
        string? countryId = null,
        string? countryCode = null,
        string? description = null)
    {
        Value = value;
        CountryId = countryId;
        CountryCode = countryCode;
        Description = description;
    }
    
    public required string Value { get; init; }
    public string? CountryId { get; init; }
    public string? CountryCode { get; init; }
    public string? Description { get; init; }
}