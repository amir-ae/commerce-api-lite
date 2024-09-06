using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Mapster;

namespace Commerce.Application.Customers.Queries.ById;

public sealed class CustomerByIdQueryHandler : IRequestHandler<CustomerByIdQuery, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(CustomerByIdQuery query, CancellationToken ct = default)
    {
        var customerId = query.CustomerId;
        var customer = await _customerRepository.ByIdAsync(customerId, ct);
            
        if (customer is null) return Error.NotFound(
            nameof(query.CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");
        
        return customer.Adapt<CustomerResponse>();
    }
}