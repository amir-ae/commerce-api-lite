using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Common.ValueObjects;

public sealed class ProductId : StronglyTypedId<string>
{
    public ProductId(string value) : base(value)
    {
    }
}