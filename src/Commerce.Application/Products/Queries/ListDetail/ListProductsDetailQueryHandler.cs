using System.Collections.Concurrent;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Mapster;

namespace Commerce.Application.Products.Queries.ListDetail;

public sealed class ListProductsDetailQueryHandler : IRequestHandler<ListProductsDetailQuery, ErrorOr<ProductResponse[]>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ListProductsDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductResponse[]>> Handle(ListProductsDetailQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var products = await _productRepository.ListDetailAsync(centreId, ct);

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

        return productResponses
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .ToArray();
    }
}