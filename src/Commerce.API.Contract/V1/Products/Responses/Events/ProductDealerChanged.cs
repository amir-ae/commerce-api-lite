using Commerce.API.Contract.V1.Products.Responses.Models;

namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductDealerChanged(
    Customer? Dealer,
    DateTimeOffset DealerChangedAt) : ProductEvent;