using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerAddressChangedEvent : CustomerEvent
{
    public CustomerAddressChangedEvent(
        CustomerId customerId,
        CityId cityId,
        string address,
        AppUserId actor,
        DateTimeOffset? addressChangedAt = null) : base(
        customerId, actor)
    {
        CityId = cityId;
        Address = address;
        AddressChangedAt = addressChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public CityId CityId { get; init; }
    public string Address { get; init; }
    public DateTimeOffset AddressChangedAt { get; init; }
}