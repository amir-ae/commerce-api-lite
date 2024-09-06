using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Application.Common.Services;

public class ValidatorChecks : IValidatorChecks
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public ValidatorChecks(ICustomerRepository customerRepository, IProductRepository productRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> CustomerExists(CustomerId customerId, CancellationToken ct = default)
    {
        return await _customerRepository.CheckByIdAsync(customerId, ct);
    }
    
    public async Task<bool> ProductExists(ProductId productId, CancellationToken ct = default)
    {
        return await _productRepository.CheckByIdAsync(productId, ct);
    }
}