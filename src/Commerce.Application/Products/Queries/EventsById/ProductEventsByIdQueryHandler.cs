using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Products;
using MediatR;
using ErrorOr;
using Mapster;

namespace Commerce.Application.Products.Queries.EventsById;

public sealed class ProductEventsByIdQueryHandler : IRequestHandler<ProductEventsByIdQuery, ErrorOr<ProductEventsResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ProductEventsByIdQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductEventsResponse>> Handle(ProductEventsByIdQuery query, CancellationToken ct = default)
    {
        var productId = query.ProductId;
        
        var productEvents = await _productRepository.EventsByIdAsync(productId, ct);

        if (productEvents is null) return Error.NotFound(
            nameof(query.ProductId), $"{nameof(Product)} events with id {productId.Value} is not found.");
        
        var result = productEvents.Adapt<ProductEventsResponse>();
        
        return await _enrichmentService.EnrichProductEvents(result, ct);
    }
}