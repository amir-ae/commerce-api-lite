using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Products.Events;
using MediatR;

namespace Commerce.Application.Products.Events;

public class ProductDealerChangedEventHandler : INotificationHandler<ProductDealerChangedEvent>
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ICachedGetService _cachedGetService;

    public ProductDealerChangedEventHandler(IEventSubscriptionManager subscriptionManager, ICachedGetService cachedGetService)
    {
        _subscriptionManager = subscriptionManager;
        _cachedGetService = cachedGetService;
    }

    public async Task Handle(ProductDealerChangedEvent notification, CancellationToken ct = default)
    {
        var dealerId = notification.DealerId;
        var previousDealerId = notification.PreviousDealerId;
        var isChangingRole = notification.IsChangingRole;
        var productId = notification.ProductId;
        var changedBy = notification.Actor;
        var changedAt = notification.DealerChangedAt;

        if (dealerId != previousDealerId && !isChangingRole)
        {
            if (previousDealerId is not null)
            {
                var previousDealer = await _cachedGetService.CustomerById(previousDealerId, ct);
                _subscriptionManager.SubscribeToCustomerEvents(previousDealer);
                previousDealer.RemoveProduct(productId, changedBy, changedAt);
                _subscriptionManager.UnsubscribeFromCustomerEvents(previousDealer);
            }
            
            if (dealerId is not null)
            {
                var dealer = await _cachedGetService.CustomerById(dealerId, ct);
                _subscriptionManager.SubscribeToCustomerEvents(dealer);
                dealer.AddProduct(productId, changedBy, changedAt);
                _subscriptionManager.UnsubscribeFromCustomerEvents(dealer);
            }
        }
    }
}