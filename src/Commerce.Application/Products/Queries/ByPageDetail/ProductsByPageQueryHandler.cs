using System.Collections.Concurrent;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Mapster;

namespace Commerce.Application.Products.Queries.ByPageDetail;

public sealed class ProductsByPageDetailQueryHandler : IRequestHandler<ProductsByPageDetailQuery, ErrorOr<PaginatedList<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ProductsByPageDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> Handle(ProductsByPageDetailQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageNumber, nextPage, keyId, centreId) = query;

        var (products, totalCount) = await _productRepository.ByPageDetailAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var productResponses = new ConcurrentBag<ProductResponse>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = ct
        };
        
        await Parallel.ForEachAsync(products, parallelOptions, async (product, token) =>
        {
            var productResponse = product.Adapt<ProductResponse>();
            productResponses.Add(await _enrichmentService.EnrichProductResponse(productResponse, token));
        });
        
        return new PaginatedList<ProductResponse>(
            pageNumber, pageSize, totalCount, productResponses
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .ToArray());
    }
}