using FluentValidation;

namespace Commerce.Application.Products.Queries.CheckById;


public sealed class CheckProductByIdQueryValidator : AbstractValidator<CheckProductByIdQuery>
{
    public CheckProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}