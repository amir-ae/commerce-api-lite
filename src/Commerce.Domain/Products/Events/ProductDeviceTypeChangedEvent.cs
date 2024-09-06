using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductDeviceTypeChangedEvent : ProductEvent
{
    public ProductDeviceTypeChangedEvent(
        ProductId productId,
        string? deviceType,
        AppUserId actor,
        DateTimeOffset? deviceTypeChangedAt = null) : base(
        productId, actor)
    {
        DeviceType = deviceType;
        DeviceTypeChangedAt = deviceTypeChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string? DeviceType { get; init; }
    public DateTimeOffset DeviceTypeChangedAt { get; init; }
}