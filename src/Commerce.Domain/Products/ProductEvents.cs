using System.Diagnostics.CodeAnalysis;
using Commerce.Domain.Products.Events;

namespace Commerce.Domain.Products;

public sealed record ProductEvents
{
    [SetsRequiredMembers]
    public ProductEvents(
        ProductCreatedEvent productCreatedEvent,
        List<ProductBrandChangedEvent>? productBrandChangedEvents = null,
        List<ProductModelChangedEvent>? productModelChangedEvents = null,
        List<ProductOwnerChangedEvent>? productOwnerChangedEvents = null,
        List<ProductDealerChangedEvent>? productDealerChangedEvents = null,
        List<ProductOrderAddedEvent>? productOrderAddedEvents = null,
        List<ProductOrderRemovedEvent>? productOrderRemovedEvents = null,
        List<ProductDeviceTypeChangedEvent>? productDeviceTypeChangedEvents = null,
        List<ProductPanelChangedEvent>? productPanelChangedEvents = null,
        List<ProductWarrantyCardNumberChangedEvent>? productWarrantyCardNumberChangedEvents = null,
        List<ProductPurchaseDataChangedEvent>? productDateOfPurchaseChangedEvents = null,
        List<ProductUnrepairableEvent>? productUnrepairableEvents = null,
        List<ProductActivatedEvent>? productActivatedEvents = null,
        List<ProductDeactivatedEvent>? productDeactivatedEvents = null,
        List<ProductDeletedEvent>? productDeletedEvents = null,
        List<ProductUndeletedEvent>? productUndeletedEvents = null)
    {
        ProductCreatedEvent = productCreatedEvent;
        ProductBrandChangedEvents = productBrandChangedEvents ?? new();
        ProductModelChangedEvents = productModelChangedEvents ?? new();
        ProductOwnerChangedEvents = productOwnerChangedEvents ?? new();
        ProductDealerChangedEvents = productDealerChangedEvents ?? new();
        ProductOrderAddedEvents = productOrderAddedEvents ?? new();
        ProductOrderRemovedEvents = productOrderRemovedEvents ?? new();
        ProductDeviceTypeChangedEvents = productDeviceTypeChangedEvents ?? new();
        ProductPanelChangedEvents = productPanelChangedEvents ?? new();
        ProductWarrantyCardNumberChangedEvents = productWarrantyCardNumberChangedEvents ?? new();
        ProductDateOfPurchaseChangedEvents = productDateOfPurchaseChangedEvents ?? new();
        ProductUnrepairableEvents = productUnrepairableEvents ?? new();
        ProductActivatedEvents = productActivatedEvents ?? new();
        ProductDeactivatedEvents = productDeactivatedEvents ?? new();
        ProductDeletedEvents = productDeletedEvents ?? new();
        ProductUndeletedEvents = productUndeletedEvents ?? new();
    }
    
    public required ProductCreatedEvent ProductCreatedEvent { get; init; }
    public List<ProductBrandChangedEvent> ProductBrandChangedEvents { get; set; }
    public List<ProductModelChangedEvent> ProductModelChangedEvents { get; set; }
    public List<ProductOwnerChangedEvent> ProductOwnerChangedEvents { get; set; }
    public List<ProductDealerChangedEvent> ProductDealerChangedEvents { get; set; }
    public List<ProductOrderAddedEvent> ProductOrderAddedEvents { get; set; }
    public List<ProductOrderRemovedEvent> ProductOrderRemovedEvents { get; set; }
    public List<ProductDeviceTypeChangedEvent> ProductDeviceTypeChangedEvents { get; set; }
    public List<ProductPanelChangedEvent> ProductPanelChangedEvents { get; set; }
    public List<ProductWarrantyCardNumberChangedEvent> ProductWarrantyCardNumberChangedEvents { get; set; }
    public List<ProductPurchaseDataChangedEvent> ProductDateOfPurchaseChangedEvents { get; set; }
    public List<ProductUnrepairableEvent> ProductUnrepairableEvents { get; set; }
    public List<ProductActivatedEvent> ProductActivatedEvents { get; set; }
    public List<ProductDeactivatedEvent> ProductDeactivatedEvents { get; set; }
    public List<ProductDeletedEvent> ProductDeletedEvents { get; set; }
    public List<ProductUndeletedEvent> ProductUndeletedEvents { get; set; }
}