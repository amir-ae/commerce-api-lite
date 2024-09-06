namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerDeleted(
    DateTimeOffset DeletedAt) : CustomerEvent;