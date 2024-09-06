using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerRoleChangedEvent : CustomerEvent, IDomainEvent
{
    public CustomerRoleChangedEvent(
        CustomerId customerId,
        CustomerRole role,
        AppUserId actor,
        DateTimeOffset? roleChangedAt = null) : base(
        customerId, actor)
    {
        Role = role;
        RoleChangedAt = roleChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public CustomerRole Role { get; init; }
    public DateTimeOffset RoleChangedAt { get; init; }
}