using Commerce.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Common.Persistence.ValueConverters;

public class AppUserIdValueConverter : ValueConverter<AppUserId, Guid>
{
    public AppUserIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new AppUserId(value),
            mappingHints
        ) { }
}