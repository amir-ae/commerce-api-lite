using System.Text.Json;
using Catalog.API.Client;
using Catalog.API.Contract.V1.Countries.Responses;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Customers.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;

namespace Commerce.Infrastructure.Common.Services;

public class LookupService : ILookupService
{
    private readonly ICatalogClient _client;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _serializerOptions;

    public LookupService(ICatalogClient client, IDistributedCache cache)
    {
        _client = client;
        _cache = cache;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<PhoneNumber?> InspectCountryCode(PhoneNumber? phoneNumber, CancellationToken ct = default)
    {
        if (phoneNumber is null)
        {
            return null;
        }
        return await phoneNumber.InspectCountryCode(CountryCode, ct);
    }

    public async ValueTask<string?> CountryCode(CountryId countryId, CancellationToken ct = default)
    {
        var country = await Country(countryId.Value, ct);
        return country?.PhoneCode;
    }

    private async ValueTask<CountryResponse?> Country(string countryId, CancellationToken ct = default)
    {
        var cacheKey = $"country_{countryId}";

        var cachedCountry = await _cache.GetStringAsync(cacheKey, token: ct);
        if (cachedCountry is not null)
        {
            return JsonSerializer.Deserialize<CountryResponse>(cachedCountry, _serializerOptions);
        }
        
        var country = await FetchCountry(countryId, ct);

        if (country is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(country, _serializerOptions), cacheOptions, token: ct);
            return country;
        }

        return null;
    }
    
    private async Task<CountryResponse?> FetchCountry(string countryId, CancellationToken ct = default)
    {
        try
        {
            var result = await _client.Country.ById(countryId, ct);
            if (!result.IsError)
            {
                return result.Value;
            }
        }
        catch
        {
            //ignore
        }
        
        return null;
    }
}