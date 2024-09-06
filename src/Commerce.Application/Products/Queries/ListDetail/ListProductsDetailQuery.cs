using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.ListDetail;

public record ListProductsDetailQuery(
    CentreId? CentreId) : IRequest<ErrorOr<ProductResponse[]>>;