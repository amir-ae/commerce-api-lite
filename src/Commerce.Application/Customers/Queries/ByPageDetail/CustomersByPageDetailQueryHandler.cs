using System.Collections.Concurrent;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Mapster;

namespace Commerce.Application.Customers.Queries.ByPageDetail;

public sealed class CustomersByPageDetailQueryHandler : IRequestHandler<CustomersByPageDetailQuery, ErrorOr<PaginatedList<CustomerResponse>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public CustomersByPageDetailQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> Handle(CustomersByPageDetailQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageNumber, nextPage, keyId, centreId) = query;

        var (customers, totalCount) = await _customerRepository.ByPageDetailAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var customerResponses = new ConcurrentBag<CustomerResponse>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = ct
        };
        
        await Parallel.ForEachAsync(customers, parallelOptions, async (customer, token) =>
        {
            var customerResponse = customer.Adapt<CustomerResponse>();
            customerResponses.Add(await _enrichmentService.EnrichCustomerResponse(customerResponse, token));
        });

        return new PaginatedList<CustomerResponse>(
            pageNumber, pageSize, totalCount, customerResponses
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .ToArray());
    }
}