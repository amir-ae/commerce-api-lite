using System.Runtime.Serialization;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductDealerChangedEvent : ProductEvent, IDomainEvent
{
    public ProductDealerChangedEvent(
        ProductId productId,
        CustomerId? dealerId,
        CustomerId? previousDealerId,
        bool isChangingRole,
        AppUserId actor,
        DateTimeOffset? dealerChangedAt = null) : base(
        productId, actor)
    {
        DealerId = dealerId;
        PreviousDealerId = previousDealerId;
        IsChangingRole = isChangingRole;
        DealerChangedAt = dealerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public CustomerId? DealerId { get; init; }
    public CustomerId? PreviousDealerId { get; init; }
    [IgnoreDataMember]
    public bool IsChangingRole { get; init; }
    public DateTimeOffset DealerChangedAt { get; init; }
}