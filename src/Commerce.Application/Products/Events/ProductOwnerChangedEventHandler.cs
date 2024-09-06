using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Products.Events;
using MediatR;

namespace Commerce.Application.Products.Events;

public class ProductOwnerChangedEventHandler : INotificationHandler<ProductOwnerChangedEvent>
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ICachedGetService _cachedGetService;

    public ProductOwnerChangedEventHandler(IEventSubscriptionManager subscriptionManager, ICachedGetService cachedGetService)
    {
        _subscriptionManager = subscriptionManager;
        _cachedGetService = cachedGetService;
    }

    public async Task Handle(ProductOwnerChangedEvent notification, CancellationToken ct = default)
    {
        var ownerId = notification.OwnerId;
        var previousOwnerId = notification.PreviousOwnerId;
        var isChangingRole = notification.IsChangingRole;
        var productId = notification.ProductId;
        var changedBy = notification.Actor;
        var changedAt = notification.OwnerChangedAt;

        if (ownerId != previousOwnerId && !isChangingRole)
        {
            if (previousOwnerId is not null)
            {
                var previousOwner = await _cachedGetService.CustomerById(previousOwnerId, ct);
                _subscriptionManager.SubscribeToCustomerEvents(previousOwner);
                previousOwner.RemoveProduct(productId, changedBy, changedAt);
                _subscriptionManager.UnsubscribeFromCustomerEvents(previousOwner);
            }
            
            if (ownerId is not null)
            {
                var owner = await _cachedGetService.CustomerById(ownerId, ct);
                _subscriptionManager.SubscribeToCustomerEvents(owner);
                owner.AddProduct(productId, changedBy, changedAt);
                _subscriptionManager.UnsubscribeFromCustomerEvents(owner);
            }
        }
    }
}