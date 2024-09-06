using Commerce.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Commerce.Application.Products.Commands.Delete;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeleteBy).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}