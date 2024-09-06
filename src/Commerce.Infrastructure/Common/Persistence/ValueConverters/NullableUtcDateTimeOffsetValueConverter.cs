using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class NullableUtcDateTimeOffsetValueConverter : ValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public NullableUtcDateTimeOffsetValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            mappingHints)
    { }
}