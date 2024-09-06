using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.API.Contract.V1.Products.Responses;

namespace Commerce.Application.Common.Interfaces.Services;

public interface IEnrichmentService
{
    ValueTask<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default);
    ValueTask<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default);
    ValueTask<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default);
    ValueTask<ProductEventsResponse> EnrichProductEvents(ProductEventsResponse productEvents, CancellationToken ct = default);
}