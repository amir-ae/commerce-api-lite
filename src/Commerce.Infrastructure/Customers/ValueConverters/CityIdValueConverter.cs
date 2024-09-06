using Commerce.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Customers.ValueConverters;

public class CityIdValueConverter : ValueConverter<CityId, int>
{
    public CityIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CityId(value),
            mappingHints
        ) { }
}