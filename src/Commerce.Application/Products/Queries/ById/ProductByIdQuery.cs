using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.ById;

public record ProductByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<ProductResponse>>;