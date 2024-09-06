using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class OrderIdValueConverter : ValueConverter<OrderId, string>
{
    public OrderIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new OrderId(value),
            mappingHints
        ) { }
}