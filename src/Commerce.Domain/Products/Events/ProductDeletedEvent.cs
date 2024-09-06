using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductDeletedEvent : ProductEvent
{
    public ProductDeletedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? deletedAt = null) : base(
        productId, actor)
    {
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeletedAt { get; init; }
}