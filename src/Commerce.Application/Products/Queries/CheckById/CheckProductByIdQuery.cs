using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.CheckById;

public record CheckProductByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<bool>>;