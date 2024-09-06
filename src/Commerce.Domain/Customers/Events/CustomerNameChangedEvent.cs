using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.Helpers;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerNameChangedEvent : CustomerEvent
{
    public CustomerNameChangedEvent(
        CustomerId customerId,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        AppUserId actor,
        DateTimeOffset? nameChangedAt = null) : base(
        customerId, actor)
    {
        (FullName, FirstName, MiddleName, LastName) = NameHelper.BuildCustomerName(firstName, middleName, lastName, fullName);
        NameChangedAt = nameChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string FullName { get; init; }
    public DateTimeOffset NameChangedAt { get; init; }
}