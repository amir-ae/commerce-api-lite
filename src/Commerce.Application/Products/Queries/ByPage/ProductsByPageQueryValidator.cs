using FluentValidation;

namespace Commerce.Application.Products.Queries.ByPage;

public sealed class ProductsByPageQueryValidator : AbstractValidator<ProductsByPageQuery>
{
    public ProductsByPageQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}