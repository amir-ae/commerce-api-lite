using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Domain.Common.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.List;

public record ListProductsQuery(
    CentreId? CentreId) : IRequest<ErrorOr<ProductBriefResponse[]>>;