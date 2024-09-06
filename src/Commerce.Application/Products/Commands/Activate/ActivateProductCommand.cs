using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Application.Products.Commands.Activate;

public record ActivateProductCommand(
    ProductId ProductId,
    AppUserId ActivateBy) : IRequest<ErrorOr<Product>>;