using MediatR;
using ErrorOr;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Application.Products.Commands.Delete;

public record DeleteProductCommand(
    ProductId ProductId,
    AppUserId DeleteBy) : IRequest<ErrorOr<Product>>;