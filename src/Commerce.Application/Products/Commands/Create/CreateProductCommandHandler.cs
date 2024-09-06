using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Application.Products.Commands.Update;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;

namespace Commerce.Application.Products.Commands.Create;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ISender _mediator;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork,
        IEventSubscriptionManager subscriptionManager,
        ISender mediator)
    {
        _productRepository = unitOfWork.ProductRepository;
        _subscriptionManager = subscriptionManager;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Product>> Handle(CreateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, serialId, ownerId, dealerId, deviceType, panelModel, 
            panelSerialNumber, warrantyCardNumber, dateOfPurchase, invoiceNumber, 
            purchasePrice, orderIds, unrepairable, dateOfDemandForCompensation, 
            demanderFullName, createBy, createAt, isPreChecked) = command;

        var product = isPreChecked ? null : await _productRepository.ByIdAsync(productId, ct);

        if (product is null)
        {
            var result = Product.Construct(productId,
                brand,
                model,
                serialId,
                ownerId,
                dealerId,
                deviceType,
                panelModel,
                panelSerialNumber,
                warrantyCardNumber,
                dateOfPurchase,
                invoiceNumber,
                purchasePrice,
                orderIds,
                unrepairable,
                dateOfDemandForCompensation,
                demanderFullName,
                createBy,
                createAt,
                _productRepository.Create);
            
            return result;
        }
        
        _subscriptionManager.SubscribeToProductEvents(product);
        product.Undelete(createBy);
            
        var updateProduct = new UpdateProductCommand(
            productId,
            brand,
            model,
            ownerId,
            dealerId,
            deviceType,
            panelModel,
            panelSerialNumber,
            warrantyCardNumber,
            dateOfPurchase,
            invoiceNumber,
            purchasePrice,
            orderIds,
            unrepairable,
            dateOfDemandForCompensation,
            demanderFullName,
            createBy,
            createAt);
                
        return await _mediator.Send(updateProduct, ct);
    }
}