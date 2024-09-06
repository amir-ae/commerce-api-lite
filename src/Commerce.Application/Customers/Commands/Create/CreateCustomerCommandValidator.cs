using Commerce.Domain.Customers.ValueObjects;
using FluentValidation;

namespace Commerce.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.CreateBy.Value).NotEmpty();
        When(x => x.CustomerId is not null, () =>
        {
            RuleFor(x => x.CustomerId!.Value).MaximumLength(36);
        });
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MiddleName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(100)
            .NotEmpty().When(x => x.Role is null || x.Role.Value == CustomerRole.Owner);
        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber.Value).NotEmpty().MaximumLength(30);
            RuleFor(x => x.PhoneNumber.CountryId.Value).MaximumLength(10);
            RuleFor(x => x.PhoneNumber.CountryCode).MaximumLength(10);
            RuleFor(x => x.PhoneNumber.Description).MaximumLength(50);
        });
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
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