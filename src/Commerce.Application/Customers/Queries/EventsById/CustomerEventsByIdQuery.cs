using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Customers.Queries.EventsById;

public record CustomerEventsByIdQuery(
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerEventsResponse>>;