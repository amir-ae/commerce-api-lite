using System.Runtime.Serialization;
using Commerce.Domain.Products;

namespace Commerce.Domain.Common.ValueObjects;

public sealed record ProductOrderLink
{
    [IgnoreDataMember]
    public ProductId ProductId { get; init; } = new(string.Empty);
    [IgnoreDataMember]
    public Product? Product { get; set; }
    public OrderId OrderId { get; init; } = null!;
    public CentreId CentreId { get; init; } = null!;
}