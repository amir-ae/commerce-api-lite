using FluentValidation;

namespace Commerce.Application.Products.Commands.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CreateBy).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        When(x => x.ProductId is not null, () =>
        {
            RuleFor(x => x.ProductId.Value).MaximumLength(50);
        });
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        When(x => x.OwnerId is not null, () =>
        {
            RuleFor(x => x.OwnerId!.Value).MaximumLength(36);
        });
        When(x => x.DealerId is not null, () =>
        {
            RuleFor(x => x.DealerId!.Value).MaximumLength(36);
        });
        RuleFor(x => x.DemanderFullName).MaximumLength(150);
        RuleFor(x => x.DeviceType).MaximumLength(30);
        RuleFor(x => x.PanelModel).MaximumLength(50);
        RuleFor(x => x.PanelSerialNumber).MaximumLength(100);
        RuleFor(x => x.WarrantyCardNumber).MaximumLength(50);
        RuleFor(x => x.InvoiceNumber).MaximumLength(50);
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        When(x => x.OrderIds is not null, () =>
        {
            RuleForEach(x => x.OrderIds).ChildRules(orderId =>
            {
                orderId.RuleFor(o => o.Value).MaximumLength(36);
            });
        });
        RuleFor(x => x.DemanderFullName).MaximumLength(150);
    }
}