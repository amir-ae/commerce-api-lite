using System.Diagnostics.CodeAnalysis;
using Commerce.API.Contract.V1.Common;
using Commerce.API.Contract.V1.Common.Models;
using Commerce.API.Contract.V1.Common.Requests;

namespace Commerce.API.Contract.V1.Customers.Requests;

public record CreateCustomerRequest : CreateRequest
{
    public CreateCustomerRequest() : base()
    {
    }
    
    [SetsRequiredMembers]
    public CreateCustomerRequest(
        Guid createBy,
        string firstName,
        string? middleName,
        string lastName,
        PhoneNumber phoneNumber,
        int cityId,
        string address,
        CustomerRole? role = null,
        IEnumerable<Product>? products = null,
        IEnumerable<Order>? orders = null,
        string? customerId = null,
        DateTimeOffset? createAt = null) : base(
        createBy,
        createAt)
    {
        CustomerId = customerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        Products = products;
        Orders = orders;
    }
    
    public string? CustomerId { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
    public CustomerRole? Role { get; init; }
    public IEnumerable<Product>? Products { get; init; }
    public IEnumerable<Order>? Orders { get; init; }
}

