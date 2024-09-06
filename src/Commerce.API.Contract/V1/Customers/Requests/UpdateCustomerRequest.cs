using System.Diagnostics.CodeAnalysis;
using Commerce.API.Contract.V1.Common.Models;
using Commerce.API.Contract.V1.Common.Requests;

namespace Commerce.API.Contract.V1.Customers.Requests;

public record UpdateCustomerRequest : UpdateRequest
{
    public UpdateCustomerRequest() : base()
    {
    }
    
    [SetsRequiredMembers]
    public UpdateCustomerRequest(
        Guid updateBy,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        PhoneNumber? phoneNumber = null,
        int? cityId = null,
        string? address = null,
        CustomerRole? role = null,
        IEnumerable<Product>? products = null,
        IEnumerable<Order>? orders = null,
        DateTimeOffset? updateAt = null) : base(
        updateBy,
        updateAt)
    {
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
    
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
    public CustomerRole? Role { get; init; }
    public IEnumerable<Product>? Products { get; init; }
    public IEnumerable<Order>? Orders { get; init; }
}