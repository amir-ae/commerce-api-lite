using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;

namespace Commerce.Application.Products.Commands.Activate;

public sealed class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public ActivateProductCommandHandler(IProductRepository productRepository, IEventSubscriptionManager subscriptionManager)
    {
        _productRepository = productRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Product>> Handle(ActivateProductCommand command, CancellationToken ct = default)
    {
        var (productId, activateBy) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");
        
        _subscriptionManager.SubscribeToProductEvents(product);
        
        product.Activate(activateBy, _productRepository.Append);
        
        _subscriptionManager.UnsubscribeFromProductEvents(product);
        
        return product;
    }
}