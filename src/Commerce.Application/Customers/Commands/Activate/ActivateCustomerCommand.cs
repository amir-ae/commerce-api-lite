using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Application.Customers.Commands.Activate;

public record ActivateCustomerCommand(
    CustomerId CustomerId,
    AppUserId ActivateBy) : IRequest<ErrorOr<Customer>>;