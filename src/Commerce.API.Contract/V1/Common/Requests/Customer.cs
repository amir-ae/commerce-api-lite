using Commerce.API.Contract.V1.Common.Models;

namespace Commerce.API.Contract.V1.Common.Requests;

public record Customer
{
    public Customer()
    {
    }
    
    public Customer(
        string? customerId,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        PhoneNumber? phoneNumber = null,
        int? cityId = null,
        string? address = null,
        CustomerRole? role = null)
    {
        CustomerId = customerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
    }
        
    public string? CustomerId { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
    public CustomerRole? Role { get; init; }
}