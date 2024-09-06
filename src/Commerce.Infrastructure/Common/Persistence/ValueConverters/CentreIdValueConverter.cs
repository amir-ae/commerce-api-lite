using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class CentreIdValueConverter : ValueConverter<CentreId, string>
{
    public CentreIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CentreId(value),
            mappingHints
        ) { }
}