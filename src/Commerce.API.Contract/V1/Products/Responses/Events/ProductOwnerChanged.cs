using Commerce.API.Contract.V1.Products.Responses.Models;

namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductOwnerChanged(
    Customer? Owner,
    DateTimeOffset OwnerChangedAt) : ProductEvent;