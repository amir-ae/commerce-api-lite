using Commerce.Domain.Customers;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Domain.Common.ValueObjects;

public sealed record CustomerProductLink
{
    public CustomerId CustomerId { get; init; } = null!;
    public Customer? Customer { get; set; }
    public ProductId ProductId { get; init; } = null!;
    public Product? Product { get; set; }
}