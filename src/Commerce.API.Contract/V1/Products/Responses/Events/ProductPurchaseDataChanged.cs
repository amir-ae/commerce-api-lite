namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductPurchaseDataChanged(
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber, 
    decimal? PurchasePrice,
    DateTimeOffset PurchaseDataChangedAt) : ProductEvent;