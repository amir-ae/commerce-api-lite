using Commerce.Domain.Common.Models;

namespace Commerce.Domain.Common.ValueObjects;

public sealed class AppUserId : StronglyTypedId<Guid>
{
    public AppUserId(Guid value) : base(value)
    {
    }
}