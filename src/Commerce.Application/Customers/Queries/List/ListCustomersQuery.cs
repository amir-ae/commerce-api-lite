using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using ErrorOr;
using MediatR;

namespace Commerce.Application.Customers.Queries.List;

public record ListCustomersQuery(
    CentreId? CentreId) : IRequest<ErrorOr<CustomerBriefResponse[]>>;