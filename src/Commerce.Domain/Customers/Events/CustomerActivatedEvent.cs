using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerActivatedEvent : CustomerEvent
{
    public CustomerActivatedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? activatedAt = null) : base(
        customerId, actor)
    {
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset ActivatedAt { get; init; }
}