
namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public abstract record CustomerEvent
{
    public required string CustomerId { get; init; }
    public required Guid Actor { get; init; }
}