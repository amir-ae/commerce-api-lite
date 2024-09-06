using Commerce.API.Contract.V1.Common.Responses;

namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerOrderAdded(
    OrderId OrderId,
    IList<OrderId> OrderIds,
    DateTimeOffset OrderAddedAt) : CustomerEvent;