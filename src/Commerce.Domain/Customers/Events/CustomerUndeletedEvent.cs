using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerUndeletedEvent : CustomerEvent
{
    public CustomerUndeletedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? undeletedAt = null) : base(
        customerId, actor)
    {
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset UndeletedAt { get; init; }
}