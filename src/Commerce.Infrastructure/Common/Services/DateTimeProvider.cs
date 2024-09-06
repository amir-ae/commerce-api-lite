using Commerce.Application.Common.Interfaces.Services;

namespace Commerce.Infrastructure.Common.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}