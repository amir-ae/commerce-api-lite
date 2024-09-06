using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Products;

namespace Commerce.Application.Common.Interfaces.Persistence;

public interface ICachedGetService
{
    public ValueTask<Customer> CustomerById(CustomerId customerId, CancellationToken ct);
    public ValueTask<Product> ProductById(ProductId productId, CancellationToken ct);
}