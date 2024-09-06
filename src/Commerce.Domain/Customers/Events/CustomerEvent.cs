using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Customers.Events;

public abstract record CustomerEvent(CustomerId CustomerId, AppUserId Actor);