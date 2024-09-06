using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;

namespace Commerce.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public ActivateCustomerCommandHandler(ICustomerRepository customerRepository, IEventSubscriptionManager subscriptionManager)
    {
        _customerRepository = customerRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Customer>> Handle(ActivateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, activateBy) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");

        _subscriptionManager.SubscribeToCustomerEvents(customer);
        
        customer.Activate(activateBy);
        
        _subscriptionManager.UnsubscribeFromCustomerEvents(customer);

        return customer;
    }
}