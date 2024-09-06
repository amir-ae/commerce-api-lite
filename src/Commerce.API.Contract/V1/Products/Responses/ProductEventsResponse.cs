using Commerce.API.Contract.V1.Products.Responses.Events;

namespace Commerce.API.Contract.V1.Products.Responses;

public record ProductEventsResponse(ProductCreated ProductCreatedEvent,
    IList<ProductBrandChanged> ProductBrandChangedEvents,
    IList<ProductModelChanged> ProductModelChangedEvents,
    IList<ProductOwnerChanged> ProductOwnerChangedEvents,
    IList<ProductDealerChanged> ProductDealerChangedEvents,
    IList<ProductOrderAdded> ProductOrderAddedEvents,
    IList<ProductOrderRemoved> ProductOrderRemovedEvents,
    IList<ProductDeviceTypeChanged> ProductDeviceTypeChangedEvents,
    IList<ProductPanelChanged> ProductPanelChangedEvents,
    IList<ProductWarrantyCardNumberChanged> ProductWarrantyCardNumberChangedEvents,
    IList<ProductPurchaseDataChanged> ProductDateOfPurchaseChangedEvents,
    IList<ProductUnrepairable> ProductUnrepairableEvents,
    IList<ProductActivated> ProductActivatedEvents,
    IList<ProductDeactivated> ProductDeactivatedEvents,
    IList<ProductDeleted> ProductDeletedEvents,
    IList<ProductUndeleted> ProductUndeletedEvents);