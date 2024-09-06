using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerDeletedEvent : CustomerEvent
{
    public CustomerDeletedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? deletedAt = null) : base(
        customerId, actor)
    {
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeletedAt { get; init; }
}