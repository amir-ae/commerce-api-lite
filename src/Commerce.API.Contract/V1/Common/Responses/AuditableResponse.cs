namespace Commerce.API.Contract.V1.Common.Responses;

public abstract record AuditableResponse(
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    int Version,
    bool IsActive,
    bool IsDeleted);