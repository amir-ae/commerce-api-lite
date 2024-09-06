using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Requests;
using Commerce.API.Contract.V1.Customers.Responses;
using ErrorOr;

namespace Commerce.API.Client.Resources.Interfaces;

public interface ICustomerResource
{
    Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPage(int? pageSize, int? pageNumber, bool? nextPage = null, 
        string? keyId = null, string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPageDetail(int? pageSize, int? pageNumber, bool? nextPage = null, 
        string? keyId = null, string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<IList<CustomerBriefResponse>>> List(string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<IList<CustomerResponse>>> ListDetail(string? centreId = null, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> ById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> DetailById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerEventsResponse>> EventsById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Update(string customerId, UpdateCustomerRequest request, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Activate(string customerId, Guid activateBy, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Deactivate(string customerId, Guid deactivateBy, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Delete(string customerId, Guid deleteBy, CancellationToken ct = default);
}