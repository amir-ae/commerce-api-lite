using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Customers.Commands.Delete;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeleteBy).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}