using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class UtcDateTimeOffsetValueConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public UtcDateTimeOffsetValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            v => v.ToUniversalTime(),  // Convert to UTC before saving
            v => v.ToUniversalTime(),  // Ensure it's UTC when reading from the database
            mappingHints)
    { }
}