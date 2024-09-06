using FluentValidation;

namespace Commerce.Application.Products.Queries.EventsById;


public sealed class ProductEventsByIdQueryValidator : AbstractValidator<ProductEventsByIdQuery>
{
    public ProductEventsByIdQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}