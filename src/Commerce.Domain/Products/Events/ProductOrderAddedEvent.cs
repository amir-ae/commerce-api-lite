using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductOrderAddedEvent : ProductEvent
{
    public ProductOrderAddedEvent(
        ProductId productId,
        OrderId orderId,
        HashSet<OrderId>? orderIds,
        AppUserId actor,
        DateTimeOffset? orderAddedAt = null) : base(
        productId, actor)
    {
        OrderId = orderId;
        OrderIds = orderIds ?? new();
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public OrderId OrderId { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public DateTimeOffset OrderAddedAt { get; init; }
}