using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductWarrantyCardNumberChangedEvent : ProductEvent
{
    public ProductWarrantyCardNumberChangedEvent(
        ProductId productId,
        string? warrantyCardNumber,
        AppUserId actor,
        DateTimeOffset? warrantyCardNumberChangedAt = null) : base(
        productId, actor)
    {
        WarrantyCardNumber = warrantyCardNumber;
        WarrantyCardNumberChangedAt = warrantyCardNumberChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset WarrantyCardNumberChangedAt { get; init; }
}