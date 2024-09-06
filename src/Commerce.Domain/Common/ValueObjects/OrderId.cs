using Commerce.Domain.Common.Models;
using Newtonsoft.Json;

namespace Commerce.Domain.Common.ValueObjects;

public sealed class OrderId : StronglyTypedId<string>
{
    public string CentreId { get; init; }
    
    [JsonConstructor]
    public OrderId(string value, string centreId) : base(value)
    {
        CentreId = centreId;
    }
    
    public OrderId(string value) : this(value, string.Empty)
    {
    }
}