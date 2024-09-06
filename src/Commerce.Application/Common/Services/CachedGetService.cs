using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.Models;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Products;
using ErrorOr;

namespace Commerce.Application.Common.Services;

public class CachedGetService : ICachedGetService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICachingService _cachingService;

    public CachedGetService(ICustomerRepository customerRepository, IProductRepository productRepository, ICachingService cachingService)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _cachingService = cachingService;
    }

    public async ValueTask<Customer> CustomerById(CustomerId customerId, CancellationToken ct)
    {
        if (_cachingService.TryGetValue<Customer>(customerId.Value, out var customer))
        {
            return customer;
        }
        
        customer = await _customerRepository.ByStreamIdAsync(customerId, ct);

        if (customer is not null)
        {
            _cachingService.Store(customerId.Value, customer);
            return customer;
        }
        
        var notFoundError = Error.NotFound(nameof(CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");
        throw new EventualConsistencyException(notFoundError, new List<Error> { notFoundError });
    }
    
    public async ValueTask<Product> ProductById(ProductId productId, CancellationToken ct)
    {
        if (_cachingService.TryGetValue<Product>(productId.Value, out var product))
        {
            return product;
        }
        
        product = await _productRepository.ByStreamIdAsync(productId, ct);

        if (product is not null)
        {
            _cachingService.Store(productId.Value, product);
            return product;
        }
        
        var notFoundError = Error.NotFound(nameof(ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");
        throw new EventualConsistencyException(notFoundError, new List<Error> { notFoundError });
    }
}