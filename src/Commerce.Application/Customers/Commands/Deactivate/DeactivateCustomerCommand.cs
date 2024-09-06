using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Application.Customers.Commands.Deactivate;

public record DeactivateCustomerCommand(
    CustomerId CustomerId,
    AppUserId DeactivateBy) : IRequest<ErrorOr<Customer>>;