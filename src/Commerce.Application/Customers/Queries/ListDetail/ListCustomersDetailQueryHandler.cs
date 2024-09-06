using System.Collections.Concurrent;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Mapster;

namespace Commerce.Application.Customers.Queries.ListDetail;

public sealed class ListCustomersDetailQueryHandler : IRequestHandler<ListCustomersDetailQuery, ErrorOr<CustomerResponse[]>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ListCustomersDetailQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<CustomerResponse[]>> Handle(ListCustomersDetailQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var customers = await _customerRepository.ListDetailAsync(centreId, ct);
        
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
        
        return customerResponses
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .ToArray();
    }
}