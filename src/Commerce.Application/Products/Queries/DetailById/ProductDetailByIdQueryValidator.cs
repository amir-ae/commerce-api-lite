using FluentValidation;

namespace Commerce.Application.Products.Queries.DetailById;

public sealed class ProductDetailByIdQueryValidator : AbstractValidator<ProductDetailByIdQuery>
{
    public ProductDetailByIdQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}