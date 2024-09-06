namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductDeviceTypeChanged(
    string? DeviceType,
    DateTimeOffset DeviceTypeChangedAt) : ProductEvent;