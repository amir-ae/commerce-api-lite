namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductUnrepairable(
    bool IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset UnrepairableAt) : ProductEvent;