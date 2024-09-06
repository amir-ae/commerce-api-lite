using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using MediatR;

namespace Commerce.Application.Customers.Events;

public class CustomerRoleChangedEventHandler : INotificationHandler<CustomerRoleChangedEvent>
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ICachedGetService _cachedGetService;

    public CustomerRoleChangedEventHandler(IEventSubscriptionManager subscriptionManager, ICachedGetService cachedGetService)
    {
        _subscriptionManager = subscriptionManager;
        _cachedGetService = cachedGetService;
    }

    public async Task Handle(CustomerRoleChangedEvent notification, CancellationToken ct = default)
    {
        var customerId = notification.CustomerId;
        var customerRole = notification.Role;
        var changedBy = notification.Actor;
        var changedAt = notification.RoleChangedAt;

        var customer = await _cachedGetService.CustomerById(customerId, ct);

        var productTasks = customer.ProductIds.Select(productId 
            => _cachedGetService.ProductById(productId, ct).AsTask());
        
        var products = (await Task.WhenAll(productTasks)).ToList();
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        
        Parallel.ForEach(products, parallelOptions, product =>
        {
            _subscriptionManager.SubscribeToProductEvents(product);
            
            if (customerRole == CustomerRole.Owner)
            {
                product.UpdateOwner(customerId, changedBy, changedAt);
            }
            else if (customerRole == CustomerRole.Dealer)
            {
                product.UpdateDealer(customerId, changedBy, changedAt);
            }
            
            _subscriptionManager.UnsubscribeFromProductEvents(product);
        });
    }
}