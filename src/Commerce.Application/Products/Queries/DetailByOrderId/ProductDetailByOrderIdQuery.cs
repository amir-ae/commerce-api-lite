using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.DetailByOrderId;

public record ProductDetailByOrderIdQuery(
    OrderId OrderId) : IRequest<ErrorOr<ProductResponse>>;