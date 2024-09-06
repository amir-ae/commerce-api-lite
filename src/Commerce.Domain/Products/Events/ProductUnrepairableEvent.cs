using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public sealed record ProductUnrepairableEvent : ProductEvent
{
    public ProductUnrepairableEvent(
        ProductId productId,
        bool isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId actor,
        DateTimeOffset? unrepairableAt = null) : base(
        productId, actor)
    {
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        UnrepairableAt = unrepairableAt ?? DateTimeOffset.UtcNow;
    }
    
    public bool IsUnrepairable { get; init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public string? DemanderFullName { get; init; }
    public DateTimeOffset UnrepairableAt { get; init; }
}