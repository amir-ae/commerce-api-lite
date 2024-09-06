using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandValidator : AbstractValidator<ActivateCustomerCommand>
{
    public ActivateCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.ActivateBy).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}