using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerOrderAddedEvent : CustomerEvent
{
    public CustomerOrderAddedEvent(
        CustomerId customerId,
        OrderId orderId,
        HashSet<OrderId>? orderIds,
        AppUserId actor,
        DateTimeOffset? orderAddedAt = null) : base(
        customerId, actor)
    {
        OrderId = orderId;
        OrderIds = orderIds ?? new();
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public OrderId OrderId { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public DateTimeOffset OrderAddedAt { get; init; }
}