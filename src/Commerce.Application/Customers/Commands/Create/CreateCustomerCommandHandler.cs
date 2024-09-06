using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Application.Customers.Commands.Update;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Helpers;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products;

namespace Commerce.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ILookupService _lookupService;
    private readonly ISender _mediator;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork,
        IEventSubscriptionManager subscriptionManager,
        ILookupService lookupService,
        ISender mediator)
    {
        _customerRepository = unitOfWork.CustomerRepository;
        _subscriptionManager = subscriptionManager;
        _lookupService = lookupService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Customer>> Handle(CreateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, firstName, middleName, lastName, phoneNumber, cityId, address, role, 
            productIds, orderIds, createBy, createAt, isPreChecked) = command;

        Customer? customer;
        string fullName = NameHelper.BuildCustomerName(firstName, middleName, lastName).FullName;
        if (customerId is not null && !isPreChecked)
        {
            customer = await _customerRepository.ByIdAsync(customerId, ct);
        }
        else
        {
            customer = await _customerRepository.ByDataAsync(fullName, phoneNumber.Value, ct);
            customerId = customer?.Id;
        }
        
        customerId ??= new CustomerId(Guid.NewGuid().ToString());
        phoneNumber = await phoneNumber.InspectCountryCode(_lookupService.CountryCode, ct);
        
        if (customer is null)
        {
            var result = Customer.Construct(
                customerId,
                firstName,
                middleName,
                lastName,
                fullName,
                phoneNumber,
                cityId,
                address,
                role,
                productIds,
                orderIds,
                createBy,
                createAt,
                _customerRepository.Create);
            
            return result;
        }

        _subscriptionManager.SubscribeToCustomerEvents(customer);
        customer.Undelete(createBy);

        var updateCustomer = new UpdateCustomerCommand(
            customerId,
            firstName,
            middleName,
            lastName,
            phoneNumber,
            cityId,
            address,
            role,
            productIds,
            orderIds,
            createBy,
            createAt,
            true);
                
        return await _mediator.Send(updateCustomer, ct);
    }
}