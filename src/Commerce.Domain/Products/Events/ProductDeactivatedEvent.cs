using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductDeactivatedEvent : ProductEvent
{
    public ProductDeactivatedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? deactivatedAt = null) : base(
        productId, actor)
    {
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeactivatedAt { get; init; }
}