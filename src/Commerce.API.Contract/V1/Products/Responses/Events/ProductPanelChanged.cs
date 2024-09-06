namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductPanelChanged(
    string? PanelModel,
    string? PanelSerialNumber,
    DateTimeOffset PanelChangedAt) : ProductEvent;