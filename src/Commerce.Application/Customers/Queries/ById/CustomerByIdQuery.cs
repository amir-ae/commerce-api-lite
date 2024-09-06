using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Customers.Queries.ById;

public record CustomerByIdQuery(
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerResponse>>;