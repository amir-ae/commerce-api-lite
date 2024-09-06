using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using ErrorOr;
using MediatR;

namespace Commerce.Application.Customers.Queries.ByPage;

public record CustomersByPageQuery(
    int PageSize,
    int PageNumber,
    bool? NextPage,
    CustomerId? KeyId,
    CentreId? CentreId) : IRequest<ErrorOr<PaginatedList<CustomerResponse>>>;