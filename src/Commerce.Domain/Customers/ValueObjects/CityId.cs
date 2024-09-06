using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Customers.ValueObjects;

public sealed class CityId : StronglyTypedId<int>
{
    public CityId(int value) : base(value)
    {
    }
}