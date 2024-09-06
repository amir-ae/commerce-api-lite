using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerDeactivatedEvent : CustomerEvent
{
    public CustomerDeactivatedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? deactivatedAt = null) : base(
        customerId, actor)
    {
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeactivatedAt { get; init; }
}