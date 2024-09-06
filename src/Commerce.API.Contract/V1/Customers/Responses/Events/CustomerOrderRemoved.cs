using Commerce.API.Contract.V1.Common.Responses;

namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerOrderRemoved(
    OrderId OrderId,
    IList<OrderId> OrderIds,
    DateTimeOffset OrderRemovedAt) : CustomerEvent;