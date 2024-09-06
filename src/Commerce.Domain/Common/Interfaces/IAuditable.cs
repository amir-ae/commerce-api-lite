using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Common.Interfaces;

public interface IAuditable
{
    public DateTimeOffset CreatedAt { get; }
    public AppUserId CreatedBy { get; }
    public DateTimeOffset? LastModifiedAt { get; }
    public AppUserId? LastModifiedBy { get; }
}