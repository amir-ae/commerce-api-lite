using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Mapster;

namespace Commerce.Application.Products.Queries.List;

public sealed class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, ErrorOr<ProductBriefResponse[]>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductBriefResponse[]>> Handle(ListProductsQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var products = await _productRepository.ListAsync(centreId, ct);

        return products
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Adapt<ProductBriefResponse[]>();
    }
}