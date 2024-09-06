using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Application.Common.Interfaces.Services;

public interface ILookupService
{
    ValueTask<string?> CountryCode(CountryId countryId, CancellationToken ct = default);
    Task<PhoneNumber?> InspectCountryCode(PhoneNumber? phoneNumber, CancellationToken ct = default);
}