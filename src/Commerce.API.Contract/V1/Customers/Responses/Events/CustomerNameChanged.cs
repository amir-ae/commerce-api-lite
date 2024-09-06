namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerNameChanged(
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    DateTimeOffset NameChangedAt) : CustomerEvent;