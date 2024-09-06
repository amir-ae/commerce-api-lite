namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductBrandChanged(
    string Brand,
    DateTimeOffset BrandChangedAt) : ProductEvent;