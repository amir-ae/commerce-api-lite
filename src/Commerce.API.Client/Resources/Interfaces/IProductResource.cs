using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Requests;
using Commerce.API.Contract.V1.Products.Responses;
using ErrorOr;

namespace Commerce.API.Client.Resources.Interfaces;

public interface IProductResource
{
    Task<ErrorOr<PaginatedList<ProductResponse>>> ByPage(int? pageSize, int? pageNumber, bool? nextPage = null, 
        string? keyId = null, string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<PaginatedList<ProductResponse>>> ByPageDetail(int? pageSize, int? pageNumber, bool? nextPage = null, 
        string? keyId = null, string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<IList<ProductBriefResponse>>> List(string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<IList<ProductResponse>>> ListDetail(string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> ById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> DetailById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductEventsResponse>> EventsById(string productId, CancellationToken ct = default);
    Task<ErrorOr<bool>> CheckById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> DetailByOrderId(string orderId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Create(CreateProductRequest request, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Update(string productId, UpdateProductRequest request, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Activate(string productId, Guid activateBy, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Deactivate(string productId, Guid deactivateBy, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Delete(string productId, Guid deleteBy, CancellationToken ct = default);
}