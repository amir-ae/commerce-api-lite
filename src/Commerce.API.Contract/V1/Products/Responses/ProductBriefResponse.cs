using Commerce.API.Contract.V1.Common.Responses;

namespace Commerce.API.Contract.V1.Products.Responses;

public record ProductBriefResponse(string Id,
    string Brand,
    string Model,
    string? OwnerId,
    string? DealerId,
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
    DateTimeOffset CreatedAt) : BriefResponse(CreatedAt);