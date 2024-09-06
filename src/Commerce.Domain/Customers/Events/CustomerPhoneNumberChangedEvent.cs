using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerPhoneNumberChangedEvent : CustomerEvent
{
    public CustomerPhoneNumberChangedEvent(
        CustomerId customerId,
        PhoneNumber phoneNumber,
        AppUserId actor,
        DateTimeOffset? phoneNumberChangedAt = null) : base(
        customerId, actor)
    {
        PhoneNumber = phoneNumber;
        PhoneNumberChangedAt = phoneNumberChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public PhoneNumber PhoneNumber { get; init; }
    public DateTimeOffset PhoneNumberChangedAt { get; init; }
}