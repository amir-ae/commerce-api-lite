using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using MediatR;

namespace Commerce.Application.Customers.Events;

public class CustomerProductRemovedEventHandler : INotificationHandler<CustomerProductRemovedEvent>
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ICachedGetService _cachedGetService;

    public CustomerProductRemovedEventHandler(IEventSubscriptionManager subscriptionManager, ICachedGetService cachedGetService)
    {
        _subscriptionManager = subscriptionManager;
        _cachedGetService = cachedGetService;
    }

    public async Task Handle(CustomerProductRemovedEvent notification, CancellationToken ct = default)
    {
        var productId = notification.ProductId;
        var customerRole = notification.CustomerRole;
        var removedBy = notification.Actor;
        var removedAt = notification.ProductRemovedAt;

        var product = await _cachedGetService.ProductById(productId, ct);
        _subscriptionManager.SubscribeToProductEvents(product);
        if (customerRole == CustomerRole.Owner)
        {
            product.UpdateOwner(null, removedBy, removedAt);
        }
        else if (customerRole == CustomerRole.Dealer)
        {
            product.UpdateDealer(null, removedBy, removedAt);
        }
        _subscriptionManager.UnsubscribeFromProductEvents(product);
    }
}