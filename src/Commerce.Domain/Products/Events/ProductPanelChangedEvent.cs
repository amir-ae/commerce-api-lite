using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductPanelChangedEvent : ProductEvent
{
    public ProductPanelChangedEvent(
        ProductId productId,
        string? panelModel,
        string? panelSerialNumber,
        AppUserId actor,
        DateTimeOffset? panelChangedAt = null) : base(
        productId, actor)
    {
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        PanelChangedAt = panelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public DateTimeOffset PanelChangedAt { get; init; }
}