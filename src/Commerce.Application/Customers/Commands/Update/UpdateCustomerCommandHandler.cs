using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Microsoft.AspNetCore.Http;

namespace Commerce.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ILookupService _lookupService;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, 
        IEventSubscriptionManager subscriptionManager, 
        ILookupService lookupService)
    {
        _customerRepository = customerRepository;
        _subscriptionManager = subscriptionManager;
        _lookupService = lookupService;

    }

    public async Task<ErrorOr<Customer>> Handle(UpdateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, firstName, middleName, lastName, phoneNumber, 
            cityId, address, role, productIds, orders, 
            updateBy, updateAt, isPreChecked, version) = command;
        
        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");
        
        if (version.HasValue && version != customer.Version) return Error.Conflict(
            nameof(StatusCodes.Status412PreconditionFailed), $"{nameof(customer.Version)} mismatch.");

        _subscriptionManager.SubscribeToCustomerEvents(customer);

        customer.UpdateName(firstName, middleName, lastName, updateBy, updateAt);

        phoneNumber = await _lookupService.InspectCountryCode(phoneNumber, ct);
        customer.UpdatePhoneNumber(phoneNumber, updateBy, updateAt);

        customer.UpdateAddress(cityId, address, updateBy, updateAt);

        customer.AddOrders(orders, updateBy, updateAt);

        customer.UpdateRole(role, updateBy, updateAt);
        
        customer.AddProducts(productIds, updateBy, updateAt);
            
        if (!isPreChecked)
        {
            customer.RemoveProducts(productIds, updateBy, updateAt);
        }

        _subscriptionManager.UnsubscribeFromCustomerEvents(customer);

        return customer;
    }
}