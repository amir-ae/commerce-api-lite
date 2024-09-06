using System.Runtime.Serialization;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerProductRemovedEvent : CustomerEvent, IDomainEvent
{
    public CustomerProductRemovedEvent(
        CustomerId customerId,
        ProductId productId,
        HashSet<ProductId>? productIds,
        CustomerRole? customerRole,
        AppUserId actor,
        DateTimeOffset? productRemovedAt = null) : base(
        customerId, actor)
    {
        ProductId = productId;
        ProductIds = productIds ?? new();
        CustomerRole = customerRole;
        ProductRemovedAt = productRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public ProductId ProductId { get; init; }
    public HashSet<ProductId> ProductIds { get; init; }
    [IgnoreDataMember]
    public CustomerRole? CustomerRole { get; init; }
    public DateTimeOffset ProductRemovedAt { get; init; }
}