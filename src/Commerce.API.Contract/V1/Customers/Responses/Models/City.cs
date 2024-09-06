namespace Commerce.API.Contract.V1.Customers.Responses.Models;

public record City(int Id,
    string? Name,
    string? Oblast,
    string? PostalCode,
    string? PhoneCode)
{
    public City(int id) : this(id, null, null, null, null)
    {
    }

    public City() : this(0)
    {
    }
}