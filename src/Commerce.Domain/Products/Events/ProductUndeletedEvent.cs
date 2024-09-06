using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductUndeletedEvent : ProductEvent
{
    public ProductUndeletedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? undeletedAt = null) : base(
        productId, actor)
    {
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset UndeletedAt { get; init; }
}