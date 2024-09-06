using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class NullableAppUserIdValueConverter : ValueConverter<AppUserId?, Guid?>
{
    public NullableAppUserIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id != null ? id.Value : null,
            value => value.HasValue ? new AppUserId(value.Value) : null,
            mappingHints
        ) { }
}