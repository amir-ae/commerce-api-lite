using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Customers.ValueObjects;

public sealed class CountryId : StronglyTypedId<string>
{
    public CountryId(string value) : base(value)
    {
    }
}