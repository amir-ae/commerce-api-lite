using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Application.Products.Commands.Upsert;

public record UpsertProductCommand(
    ProductId ProductId,
    string? Brand,
    string? Model,
    SerialId? SerialId,
    CustomerId? OwnerId,
    CustomerId? DealerId,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    HashSet<OrderId>? OrderIds,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    AppUserId UpsertBy,
    DateTimeOffset? UpsertAt) : IRequest<ErrorOr<Product>>;