using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.ValueObjects;

namespace Commerce.Application.Customers.Commands.Create;

public record CreateCustomerCommand(
    CustomerId? CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    PhoneNumber PhoneNumber,
    CityId CityId,
    string Address,
    CustomerRole? Role,
    HashSet<ProductId>? ProductIds,
    HashSet<OrderId>? OrderIds,
    AppUserId CreateBy,
    DateTimeOffset? CreateAt,
    bool IsPreChecked = false) : IRequest<ErrorOr<Customer>>;