using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Products.Commands.Create;
using Commerce.Application.Products.Commands.Update;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;
using FluentValidation;
using Mapster;

namespace Commerce.Application.Products.Commands.Upsert;

public sealed class UpsertProductCommandHandler : IRequestHandler<UpsertProductCommand, ErrorOr<Product>>
{
    private readonly ISender _mediator;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductCommand> _createValidator;

    public UpsertProductCommandHandler(ISender mediator, 
        IProductRepository productRepository, 
        IValidator<CreateProductCommand> createValidator)
    {
        _mediator = mediator;
        _productRepository = productRepository;
        _createValidator = createValidator;
    }

    public async Task<ErrorOr<Product>> Handle(UpsertProductCommand command, CancellationToken ct = default)
    {
        var productId = command.ProductId;
        var brand = command.Brand;
        var model = command.Model;

        if ((string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model))
            && await _productRepository.CheckByIdAsync(productId, ct))
        {
            var updateProductCommand = command.Adapt<UpdateProductCommand>() with { IsPreChecked = true };
            return await _mediator.Send(updateProductCommand, ct);
        }

        var createProductCommand = command.Adapt<CreateProductCommand>();
        
        var validationResult = await _createValidator.ValidateAsync(createProductCommand, ct);
        if (!validationResult.IsValid)
        {
            return Error.Validation(
                nameof(ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");
        }
        
        return await _mediator.Send(createProductCommand, ct);
    }
}