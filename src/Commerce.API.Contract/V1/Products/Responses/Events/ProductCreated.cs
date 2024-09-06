using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Responses.Models;

namespace Commerce.API.Contract.V1.Products.Responses.Events;

public record ProductCreated(
    string Brand,
    string Model,
    int? SerialId,
    Customer? Owner,
    Customer? Dealer,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    IList<OrderId> OrderIds,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset CreatedAt) : ProductEvent;