using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Commerce.Application.Products.Queries.ByPageDetail;

public record ProductsByPageDetailQuery(
    int PageSize,
    int PageNumber,
    bool? NextPage,
    ProductId? KeyId,
    CentreId? CentreId) : IRequest<ErrorOr<PaginatedList<ProductResponse>>>;