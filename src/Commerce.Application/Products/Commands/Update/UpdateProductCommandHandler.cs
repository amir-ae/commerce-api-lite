using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Commerce.Domain.Products;
using Microsoft.AspNetCore.Http;

namespace Commerce.Application.Products.Commands.Update;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public UpdateProductCommandHandler(IProductRepository productRepository, 
        IEventSubscriptionManager subscriptionManager)
    {
        _productRepository = productRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Product>> Handle(UpdateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, ownerId, dealerId, deviceType, panelModel, 
            panelSerialNumber, warrantyCardNumber, dateOfPurchase, invoiceNumber, 
            purchasePrice, orderIds, isUnrepairable, dateOfDemandForCompensation, 
            demanderFullName, updateBy, updateAt, isPreChecked, version) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");
        
        if (version.HasValue && version != product.Version) return Error.Conflict(
            nameof(StatusCodes.Status412PreconditionFailed), $"{nameof(product.Version)} mismatch.");
        
        _subscriptionManager.SubscribeToProductEvents(product);

        product.UpdateBrand(brand, updateBy, updateAt);

        product.UpdateModel(model, updateBy, updateAt);

        product.UpdateDeviceType(deviceType, updateBy, updateAt);

        product.UpdatePanel(panelModel, panelSerialNumber, updateBy, updateAt);

        product.UpdateWarrantyCardNumber(warrantyCardNumber, updateBy, updateAt);
        
        product.UpdatePurchaseData(dateOfPurchase, invoiceNumber, purchasePrice, updateBy, updateAt);

        product.UpdateUnrepairable(isUnrepairable, dateOfDemandForCompensation, demanderFullName, 
            updateBy, updateAt);

        product.AddOrders(orderIds, updateBy, updateAt);

        product.UpdateOwner(ownerId, updateBy, updateAt);

        product.UpdateDealer(dealerId, updateBy, updateAt);
        
        _subscriptionManager.UnsubscribeFromProductEvents(product);

        return product;
    }
}