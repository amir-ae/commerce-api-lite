using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Application.Common.Interfaces.Persistence;

public interface IValidatorChecks
{
    Task<bool> CustomerExists(CustomerId customerId, CancellationToken ct = default);
    Task<bool> ProductExists(ProductId productId, CancellationToken ct = default);
}