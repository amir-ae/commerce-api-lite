using System.Diagnostics.CodeAnalysis;

namespace Commerce.API.Contract.V1.Common.Requests;

public record Order
{
    public Order()
    {
    }

    [SetsRequiredMembers]
    public Order(
        string orderId,
        string centreId)
    {
        OrderId = orderId;
        CentreId = centreId;
    }
        
    public required string OrderId { get; init; }
    public required string CentreId { get; init; }
}