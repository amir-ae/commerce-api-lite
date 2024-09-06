using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;
using Mapster;

namespace Commerce.Application.Products.Queries.ById;

public sealed class ProductByIdQueryHandler : IRequestHandler<ProductByIdQuery, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public ProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(ProductByIdQuery query, CancellationToken ct = default)
    {
        var productId = query.ProductId;
        var product = await _productRepository.ByIdAsync(productId, ct);
        
        if (product is null) return Error.NotFound(
            nameof(query.ProductId), $"{nameof(Product)} with id {productId.Value} is not found.");
        
        return product.Adapt<ProductResponse>();
    }
}