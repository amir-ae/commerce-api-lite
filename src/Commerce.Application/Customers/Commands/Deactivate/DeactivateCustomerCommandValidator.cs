using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Customers.Commands.Deactivate;

public sealed class DeactivateCustomerCommandValidator : AbstractValidator<DeactivateCustomerCommand>
{
    public DeactivateCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeactivateBy).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}