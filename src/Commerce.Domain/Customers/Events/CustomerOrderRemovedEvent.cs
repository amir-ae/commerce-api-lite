using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;
public sealed record CustomerOrderRemovedEvent : CustomerEvent
{
    public CustomerOrderRemovedEvent(
        CustomerId customerId,
        OrderId orderId,
        HashSet<OrderId>? orderIds,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        customerId, actor)
    {
        OrderId = orderId;
        OrderIds = orderIds ?? new();
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public OrderId OrderId { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public DateTimeOffset OrderRemovedAt { get; init; }
}