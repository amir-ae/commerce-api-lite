using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Customers.Commands.Create;
using Commerce.Application.Customers.Commands.Update;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using FluentValidation;
using Mapster;

namespace Commerce.Application.Customers.Commands.Upsert;

public sealed class UpsertCustomerCommandHandler : IRequestHandler<UpsertCustomerCommand, ErrorOr<Customer>>
{
    private readonly ISender _mediator;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerCommand> _createValidator;

    public UpsertCustomerCommandHandler(ISender mediator, 
        ICustomerRepository customerRepository, 
        IValidator<CreateCustomerCommand> createValidator)
    {
        _mediator = mediator;
        _customerRepository = customerRepository;
        _createValidator = createValidator;
    }

    public async Task<ErrorOr<Customer>> Handle(UpsertCustomerCommand command, CancellationToken ct = default)
    {
        var customerId = command.CustomerId;

        if (customerId is not null && await _customerRepository.CheckByIdAsync(customerId, ct))
        {
            var updateCustomerCommand = command.Adapt<UpdateCustomerCommand>();
            return await _mediator.Send(updateCustomerCommand, ct);
        }

        var createCustomerCommand = command.Adapt<CreateCustomerCommand>();

        if (customerId is not null)
        {
            var validationResult = await _createValidator.ValidateAsync(createCustomerCommand, ct);
            if (!validationResult.IsValid)
            {
                return Error.Validation(
                    nameof(CustomerId), $"{nameof(Customer)} with id {customerId.Value} is not found.");
            }
        }
        
        return await _mediator.Send(createCustomerCommand, ct);
    }
}