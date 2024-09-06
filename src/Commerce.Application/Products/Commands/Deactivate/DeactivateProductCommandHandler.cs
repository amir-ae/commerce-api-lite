using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;

namespace Commerce.Application.Products.Commands.Deactivate;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public DeactivateProductCommandHandler(IProductRepository productRepository, IEventSubscriptionManager subscriptionManager)
    {
        _productRepository = productRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Product>> Handle(DeactivateProductCommand command, CancellationToken ct = default)
    {
        var (productId, deactivateBy) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");

        _subscriptionManager.SubscribeToProductEvents(product);
        
        product.Deactivate(deactivateBy, _productRepository.Append);
        
        _subscriptionManager.UnsubscribeFromProductEvents(product);
   
        return product;
    }
}