using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Application.Products.Commands.Deactivate;

public record DeactivateProductCommand(
    ProductId ProductId,
    AppUserId DeactivateBy) : IRequest<ErrorOr<Product>>;