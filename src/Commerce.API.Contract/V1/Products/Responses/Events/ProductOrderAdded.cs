using Commerce.API.Contract.V1.Common.Responses;

namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductOrderAdded(
    OrderId OrderId,
    IList<OrderId> OrderIds,
    DateTimeOffset OrderAddedAt) : ProductEvent;