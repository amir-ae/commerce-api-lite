using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductModelChangedEvent : ProductEvent
{
    public ProductModelChangedEvent(
        ProductId productId,
        string model,
        AppUserId actor,
        DateTimeOffset? modelChangedAt = null) : base(
        productId, actor)
    {
        Model = model;
        ModelChangedAt = modelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string Model { get; init; }
    public DateTimeOffset ModelChangedAt { get; init; }
}