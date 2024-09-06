using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductCreatedEvent : ProductEvent
{
    public ProductCreatedEvent(
        ProductId productId,
        string brand,
        string model,
        SerialId? serialId,
        CustomerId? ownerId,
        CustomerId? dealerId,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        HashSet<OrderId>? orderIds,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId actor,
        DateTimeOffset? createdAt = null) : base(
        productId, actor)
    {
        Brand = brand;
        Model = model;
        SerialId = serialId;
        OwnerId = ownerId;
        DealerId = dealerId;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        OrderIds = orderIds ?? new();
        IsUnrepairable = isUnrepairable ?? false;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public string Brand { get; init; }
    public string Model { get; init; }
    public SerialId? SerialId { get; init; }
    public CustomerId? OwnerId { get; init; }
    public CustomerId? DealerId { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public HashSet<OrderId> OrderIds { get; init; }
    public bool IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
}