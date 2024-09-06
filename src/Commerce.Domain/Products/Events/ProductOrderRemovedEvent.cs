using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductOrderRemovedEvent : ProductEvent
{
    public ProductOrderRemovedEvent(
        ProductId productId,
        OrderId orderId,
        HashSet<OrderId>? orderIds,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        productId, actor)
    {
        OrderId = orderId;
        OrderIds = orderIds ?? new();
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public OrderId OrderId { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public DateTimeOffset OrderRemovedAt { get; init; }
}