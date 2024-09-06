using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Application.Customers.Commands.Delete;

public record DeleteCustomerCommand(
    CustomerId CustomerId,
    AppUserId DeleteBy) : IRequest<ErrorOr<Customer>>;