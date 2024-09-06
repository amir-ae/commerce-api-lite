using System.Diagnostics.CodeAnalysis;

namespace Commerce.API.Contract.V1.Common.Requests;

public abstract record CreateRequest
{
    protected CreateRequest()
    {
    }

    [SetsRequiredMembers]
    protected CreateRequest(
        Guid createBy,
        DateTimeOffset? createAt)
    {
        CreateBy = createBy;
        CreateAt = createAt;
    }
    
    public required Guid CreateBy { get; init; }
    public DateTimeOffset? CreateAt { get; init; }
}
