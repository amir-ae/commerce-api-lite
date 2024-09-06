using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.Helpers;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public sealed record CustomerCreatedEvent : CustomerEvent
{
    public CustomerCreatedEvent(
        CustomerId customerId,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<OrderId>? orderIds,
        AppUserId actor,
        DateTimeOffset? createdAt = null) : base(
        customerId, actor)
    {
        (FullName, FirstName, MiddleName, LastName) = NameHelper.BuildCustomerName(firstName, middleName, lastName, fullName);
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role ?? CustomerRole.Owner;
        ProductIds = productIds ?? new();
        OrderIds = orderIds ?? new();
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
    
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string FullName { get; init; }
    public PhoneNumber PhoneNumber { get; init; }
    public CityId CityId { get; init; }
    public string Address { get; init; }
    public CustomerRole Role { get; init; }
    public HashSet<ProductId> ProductIds { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}