using FluentValidation;

namespace Commerce.Application.Customers.Commands.Upsert;

public sealed class UpsertCustomerCommandValidator : AbstractValidator<UpsertCustomerCommand>
{
    public UpsertCustomerCommandValidator()
    {
        RuleFor(x => x.UpsertBy).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.MiddleName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(100);
        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber!.Value).NotEmpty().MaximumLength(30);
            RuleFor(x => x.PhoneNumber!.CountryId.Value).MaximumLength(10);
            RuleFor(x => x.PhoneNumber!.CountryCode).MaximumLength(10);
            RuleFor(x => x.PhoneNumber!.Description).MaximumLength(50);
        });
        RuleFor(x => x.Address).MaximumLength(255);
        When(x => x.ProductIds is not null, () =>
        {
            RuleForEach(x => x.ProductIds).ChildRules(productId =>
            {
                productId.RuleFor(p => p.Value).MaximumLength(50);
            });
        });
        When(x => x.OrderIds is not null, () =>
        {
            RuleForEach(x => x.OrderIds).ChildRules(orderId =>
            {
                orderId.RuleFor(o => o.Value).MaximumLength(36);
            });
        });
    }
}