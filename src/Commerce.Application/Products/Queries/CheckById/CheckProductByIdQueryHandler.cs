using Commerce.Application.Common.Interfaces.Persistence;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.CheckById;

public sealed class CheckProductByIdQueryHandler : IRequestHandler<CheckProductByIdQuery, ErrorOr<bool>>
{
    private readonly IProductRepository _productRepository;

    public CheckProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<bool>> Handle(CheckProductByIdQuery query, CancellationToken ct = default)
    {
        return await _productRepository.CheckByIdAsync(query.ProductId, ct);
    }
}