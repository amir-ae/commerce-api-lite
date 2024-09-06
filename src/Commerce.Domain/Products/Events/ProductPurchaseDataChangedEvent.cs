using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductPurchaseDataChangedEvent : ProductEvent
{
    public ProductPurchaseDataChangedEvent(
        ProductId productId,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber, 
        decimal? purchasePrice,
        AppUserId actor,
        DateTimeOffset? purchaseDataChangedAt = null) : base(
        productId, actor)
    {
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        PurchaseDataChangedAt = purchaseDataChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public DateTimeOffset PurchaseDataChangedAt { get; init; }
}