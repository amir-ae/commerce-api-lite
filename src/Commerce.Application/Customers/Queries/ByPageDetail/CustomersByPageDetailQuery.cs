using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using ErrorOr;
using MediatR;

namespace Commerce.Application.Customers.Queries.ByPageDetail;

public record CustomersByPageDetailQuery(
    int PageSize,
    int PageNumber,
    bool? NextPage,
    CustomerId? KeyId,
    CentreId? CentreId) : IRequest<ErrorOr<PaginatedList<CustomerResponse>>>;