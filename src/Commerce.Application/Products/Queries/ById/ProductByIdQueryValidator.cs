using FluentValidation;

namespace Commerce.Application.Products.Queries.ById;

public sealed class ProductByIdQueryValidator : AbstractValidator<ProductByIdQuery>
{
    public ProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}