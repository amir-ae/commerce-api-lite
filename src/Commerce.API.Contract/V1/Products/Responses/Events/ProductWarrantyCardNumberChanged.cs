namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductWarrantyCardNumberChanged(
    string? WarrantyCardNumber,
    DateTimeOffset WarrantyCardNumberChangedAt) : ProductEvent;