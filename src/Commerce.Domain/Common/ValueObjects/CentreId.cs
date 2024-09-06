using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Common.ValueObjects;

public sealed class CentreId : StronglyTypedId<string>
{
    public CentreId(string value) : base(value)
    {
    }
}