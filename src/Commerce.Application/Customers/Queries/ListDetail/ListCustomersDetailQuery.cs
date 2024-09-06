using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Customers.Queries.ListDetail;

public record ListCustomersDetailQuery(
    CentreId? CentreId) : IRequest<ErrorOr<CustomerResponse[]>>;