using System.Runtime.Serialization;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductOwnerChangedEvent : ProductEvent, IDomainEvent
{
    public ProductOwnerChangedEvent(
        ProductId productId,
        CustomerId? ownerId,
        CustomerId? previousOwnerId,
        bool isChangingRole,
        AppUserId actor,
        DateTimeOffset? ownerChangedAt = null) : base(
        productId, actor)
    {
        OwnerId = ownerId;
        PreviousOwnerId = previousOwnerId;
        IsChangingRole = isChangingRole;
        OwnerChangedAt = ownerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public CustomerId? OwnerId { get; init; }
    public CustomerId? PreviousOwnerId { get; init; }
    [IgnoreDataMember]
    public bool IsChangingRole { get; init; }
    public DateTimeOffset OwnerChangedAt { get; init; }
}