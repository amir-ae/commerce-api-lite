using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Common.ValueObjects;

public sealed class CustomerId : StronglyTypedId<string>
{
    public CustomerId(string value) : base(value)
    {
    }
}