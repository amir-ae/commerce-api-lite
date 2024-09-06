using System.Runtime.Serialization;
using Commerce.Domain.Customers;

namespace Commerce.Domain.Common.ValueObjects;

public sealed record CustomerOrderLink
{
    [IgnoreDataMember]
    public CustomerId CustomerId { get; init; } = new(string.Empty);
    [IgnoreDataMember]
    public Customer? Customer { get; set; }
    public OrderId OrderId { get; init; } = null!;
    public CentreId CentreId { get; init; } = null!;
}