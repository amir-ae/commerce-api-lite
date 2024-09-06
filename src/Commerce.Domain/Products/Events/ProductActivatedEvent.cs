using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductActivatedEvent : ProductEvent
{
    public ProductActivatedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? activatedAt = null) : base(
        productId, actor)
    {
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset ActivatedAt { get; init; }
}