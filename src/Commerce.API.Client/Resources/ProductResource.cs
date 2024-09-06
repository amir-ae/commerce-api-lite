using Commerce.API.Client.Base;
using Commerce.API.Client.Resources.Interfaces;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Requests;
using Commerce.API.Contract.V1.Products.Responses;
using Products = Commerce.API.Contract.V1.Routes.Products;
using ErrorOr;

namespace Commerce.API.Client.Resources;

public class ProductResource : IProductResource
{
    private readonly IBaseClient _client;

    public ProductResource(IBaseClient client)
    {
        _client = client;
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> ByPage(int? pageSize, int? pageNumber, 
        bool? nextPage = null, string? keyId = null, string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageNumber.HasValue) queries.Add(nameof(pageNumber), pageNumber.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Products.ByPage.Uri(), queries);
        return await _client.Get<PaginatedList<ProductResponse>>(uri, ct);
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> ByPageDetail(int? pageSize, int? pageNumber, 
        bool? nextPage = null, string? keyId = null, string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageNumber.HasValue) queries.Add(nameof(pageNumber), pageNumber.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Products.ByPageDetail.Uri(), queries);
        return await _client.Get<PaginatedList<ProductResponse>>(uri, ct);
    }

    public async Task<ErrorOr<IList<ProductBriefResponse>>> List(string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Products.List.Uri(), queries);
        return await _client.Get<IList<ProductBriefResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<IList<ProductResponse>>> ListDetail(string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Products.ListDetail.Uri(), queries);
        return await _client.Get<IList<ProductResponse>>(uri, ct);
    }

    public async Task<ErrorOr<ProductResponse>> ById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.ById.Uri(productId));
        return await _client.Get<ProductResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> DetailById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.DetailById.Uri(productId));
        return await _client.Get<ProductResponse>(uri, ct);
    }

    public async Task<ErrorOr<ProductEventsResponse>> EventsById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.EventsById.Uri(productId));
        return await _client.Get<ProductEventsResponse>(uri, ct);
    }

    public async Task<ErrorOr<bool>> CheckById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Check.Uri(productId));
        return await _client.Get<bool>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> DetailByOrderId(string orderId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.DetailByOrderId.Uri(orderId));
        return await _client.Get<ProductResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> Create(CreateProductRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Create.Uri());
        return await _client.PostAsJson<ProductResponse, CreateProductRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Update(string productId, UpdateProductRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Update.Uri(productId));
        return await _client.PatchAsJson<ProductResponse, UpdateProductRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Activate(string productId, Guid activateBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(activateBy), activateBy.ToString() } };
        var uri = _client.BuildUri(Products.Activate.Uri(productId), queries);
        return await _client.Patch<ProductResponse>(uri, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Deactivate(string productId, Guid deactivateBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(deactivateBy), deactivateBy.ToString() } };
        var uri = _client.BuildUri(Products.Deactivate.Uri(productId), queries);
        return await _client.Patch<ProductResponse>(uri, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Delete(string productId, Guid deleteBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(deleteBy), deleteBy.ToString() } };
        var uri = _client.BuildUri(Products.Delete.Uri(productId), queries);
        return await _client.Delete<ProductResponse>(uri, ct);
    }
}