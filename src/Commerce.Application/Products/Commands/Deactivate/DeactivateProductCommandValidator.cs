using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Products.Commands.Deactivate;

public sealed class DeactivateProductCommandValidator : AbstractValidator<DeactivateProductCommand>
{
    public DeactivateProductCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeactivateBy).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}