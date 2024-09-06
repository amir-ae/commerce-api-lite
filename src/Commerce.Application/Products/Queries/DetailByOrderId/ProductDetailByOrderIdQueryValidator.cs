using FluentValidation;

namespace Commerce.Application.Products.Queries.DetailByOrderId;

public sealed class ProductDetailByOrderIdQueryValidator : AbstractValidator<ProductDetailByOrderIdQuery>
{
    public ProductDetailByOrderIdQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}