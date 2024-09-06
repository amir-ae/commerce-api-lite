using Commerce.API.Contract.V1.Common.Models;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses.Models;
using Commerce.API.Contract.V1.Products.Responses;

namespace Commerce.API.Contract.V1.Customers.Responses;

public record CustomerResponse(string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    PhoneNumber PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<ProductBriefResponse>? Products,
    IList<OrderId> OrderIds,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    int Version,
    bool IsActive,
    bool IsDeleted) : AuditableResponse(CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    Version,
    IsActive, 
    IsDeleted);