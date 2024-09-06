
namespace Commerce.API.Contract.V1.Products.Responses.Events;

public abstract record ProductEvent
{
    public required string ProductId { get; init; }
    public required Guid Actor { get; init; }
}