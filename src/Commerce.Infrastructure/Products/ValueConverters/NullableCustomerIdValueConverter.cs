using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Products.ValueConverters;

public class NullableCustomerIdValueConverter : ValueConverter<CustomerId?, string?>
{
    public NullableCustomerIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id == null ? null : id.Value,
            value => string.IsNullOrEmpty(value) ? null : new CustomerId(value),
            mappingHints
        ) { }
}