namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductUndeleted(
    DateTimeOffset UndeletedAt) : ProductEvent;