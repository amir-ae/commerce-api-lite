using FluentValidation;

namespace Commerce.Application.Products.Queries.ByPageDetail;

public sealed class ProductsByPageDetailQueryValidator : AbstractValidator<ProductsByPageDetailQuery>
{
    public ProductsByPageDetailQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}