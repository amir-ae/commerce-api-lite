using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Mapster;

namespace Commerce.Application.Customers.Queries.DetailById;

public sealed class CustomerDetailByIdQueryHandler : IRequestHandler<CustomerDetailByIdQuery, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public CustomerDetailByIdQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(CustomerDetailByIdQuery query, CancellationToken ct = default)
    {
        var customerId = query.CustomerId;
        var customer = await _customerRepository.DetailByIdAsync(customerId, ct);
            
        if (customer is null) return Error.NotFound(
            nameof(query.CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");

        var customerResult = customer.Adapt<CustomerResponse>();
        
        return await _enrichmentService.EnrichCustomerResponse(customerResult, ct);
    }
}