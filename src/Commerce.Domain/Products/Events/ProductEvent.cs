using Commerce.Domain.Common.ValueObjects;

namespace Commerce.Domain.Products.Events;

public abstract record ProductEvent(ProductId ProductId,
    AppUserId Actor);