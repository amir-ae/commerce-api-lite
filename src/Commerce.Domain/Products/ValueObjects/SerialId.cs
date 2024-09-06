using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Products.ValueObjects;

public sealed class SerialId : StronglyTypedId<int>
{
    public SerialId(int value) : base(value)
    {
    }
}