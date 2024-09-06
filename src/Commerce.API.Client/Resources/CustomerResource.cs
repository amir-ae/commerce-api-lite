using Commerce.API.Client.Base;
using Commerce.API.Client.Resources.Interfaces;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Requests;
using Commerce.API.Contract.V1.Customers.Responses;
using Customers = Commerce.API.Contract.V1.Routes.Customers;
using ErrorOr;

namespace Commerce.API.Client.Resources;

public class CustomerResource : ICustomerResource
{
    private readonly IBaseClient _client;

    public CustomerResource(IBaseClient client)
    {
        _client = client;
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPage(int? pageSize, int? pageNumber, 
        bool? nextPage = null, string? keyId = null, string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageNumber.HasValue) queries.Add(nameof(pageNumber), pageNumber.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Customers.ByPage.Uri(), queries);
        return await _client.Get<PaginatedList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPageDetail(int? pageSize, int? pageNumber, 
        bool? nextPage = null, string? keyId = null, string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageNumber.HasValue) queries.Add(nameof(pageNumber), pageNumber.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);

        var uri = _client.BuildUri(Customers.ByPageDetail.Uri(), queries);
        return await _client.Get<PaginatedList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<IList<CustomerBriefResponse>>> List(string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);
        
        var uri = _client.BuildUri(Customers.List.Uri(), queries);
        return await _client.Get<IList<CustomerBriefResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<IList<CustomerResponse>>> ListDetail(string? centreId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(centreId)) queries.Add(nameof(centreId), centreId);
        
        var uri = _client.BuildUri(Customers.ListDetail.Uri(), queries);
        return await _client.Get<IList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> ById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.ById.Uri(customerId));
        return await _client.Get<CustomerResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<CustomerResponse>> DetailById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.DetailById.Uri(customerId));
        return await _client.Get<CustomerResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<CustomerEventsResponse>> EventsById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.EventsById.Uri(customerId));
        return await _client.Get<CustomerEventsResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.Create.Uri());
        return await _client.PostAsJson<CustomerResponse, CreateCustomerRequest>(uri, request, ct);
    }
    
    public async Task<ErrorOr<CustomerResponse>> Update(string customerId, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.Update.Uri(customerId));
        return await _client.PatchAsJson<CustomerResponse, UpdateCustomerRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Activate(string customerId, Guid activateBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(activateBy), activateBy.ToString() } };
        var uri = _client.BuildUri(Customers.Activate.Uri(customerId), queries);
        return await _client.Patch<CustomerResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Deactivate(string customerId, Guid deactivateBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(deactivateBy), deactivateBy.ToString() } };
        var uri = _client.BuildUri(Customers.Deactivate.Uri(customerId), queries);
        return await _client.Patch<CustomerResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Delete(string customerId, Guid deleteBy, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(deleteBy), deleteBy.ToString() } };
        var uri = _client.BuildUri(Customers.Delete.Uri(customerId), queries);
        return await _client.Delete<CustomerResponse>(uri, ct);
    }
}