using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class CustomerIdValueConverter : ValueConverter<CustomerId, string>
{
    public CustomerIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CustomerId(value),
            mappingHints
        ) { }
}