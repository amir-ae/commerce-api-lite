using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductBrandChangedEvent : ProductEvent
{
    public ProductBrandChangedEvent(
        ProductId productId,
        string brand,
        AppUserId actor,
        DateTimeOffset? brandChangedAt = null) : base(
        productId, actor)
    {
        Brand = brand;
        BrandChangedAt = brandChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string Brand { get; init; }
    public DateTimeOffset BrandChangedAt { get; init; }
}