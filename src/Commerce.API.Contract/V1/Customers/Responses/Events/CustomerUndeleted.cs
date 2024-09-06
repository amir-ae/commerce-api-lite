namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerUndeleted(
    DateTimeOffset UndeletedAt) : CustomerEvent;