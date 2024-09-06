using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using MediatR;

namespace Commerce.Application.Customers.Events;

public class CustomerProductAddedEventHandler : INotificationHandler<CustomerProductAddedEvent>
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ICachedGetService _cachedGetService;

    public CustomerProductAddedEventHandler(IEventSubscriptionManager subscriptionManager, ICachedGetService cachedGetService)
    {
        _subscriptionManager = subscriptionManager;
        _cachedGetService = cachedGetService;
    }

    public async Task Handle(CustomerProductAddedEvent notification, CancellationToken ct = default)
    {
        var customerId = notification.CustomerId;
        var productId = notification.ProductId;
        var customerRole = notification.CustomerRole;
        var addedBy = notification.Actor;
        var addedAt = notification.ProductAddedAt;

        var product = await _cachedGetService.ProductById(productId, ct);
        _subscriptionManager.SubscribeToProductEvents(product);
        if (customerRole == CustomerRole.Owner)
        {
            product.UpdateOwner(customerId, addedBy, addedAt);
        }
        else if (customerRole == CustomerRole.Dealer)
        {
            product.UpdateDealer(customerId, addedBy, addedAt);
        }
        _subscriptionManager.UnsubscribeFromProductEvents(product);
    }
}