using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Products.Commands.Activate;

public sealed class ActivateProductCommandValidator : AbstractValidator<ActivateProductCommand>
{
    public ActivateProductCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.ActivateBy).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}