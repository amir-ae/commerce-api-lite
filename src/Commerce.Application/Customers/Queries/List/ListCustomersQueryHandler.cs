using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Mapster;

namespace Commerce.Application.Customers.Queries.List;

public sealed class ListCustomersQueryHandler : IRequestHandler<ListCustomersQuery, ErrorOr<CustomerBriefResponse[]>>
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerBriefResponse[]>> Handle(ListCustomersQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var customers = await _customerRepository.ListAsync(centreId, ct);

        return customers
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Adapt<CustomerBriefResponse[]>();
    }
}