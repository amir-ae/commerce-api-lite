using Commerce.API.Contract.V1.Common.Models;
using Commerce.API.Contract.V1.Customers.Responses.Models;

namespace Commerce.API.Contract.V1.Products.Responses.Models;

public record Customer(string Id,
    string? Name,
    CustomerRole? Role,
    string? FirstName,
    string? MiddleName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    City? City,
    string? Address)
{
    public Customer(string id) : this(id, 
        null, null, null, null, null, null, null, null)
    {
    }
    
    public Customer() : this(string.Empty)
    {
    }
};