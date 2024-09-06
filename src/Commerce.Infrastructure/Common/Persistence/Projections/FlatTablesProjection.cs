using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure.Common.Persistence.Projections;

public class FlatTablesProjection : IProjection
{
    private readonly Dictionary<Type, Func<object, CancellationToken, ValueTask>> _eventHandlerDefinitions;
    private readonly CommerceDbContext _dbContext;
    private readonly ICachingService _cache;

    public FlatTablesProjection(CommerceDbContext dbContext, ICachingService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        _eventHandlerDefinitions = InitializeEventHandlerDefinitions();
    }

    private Dictionary<Type, Func<object, CancellationToken, ValueTask>> InitializeEventHandlerDefinitions()
    {
        return new Dictionary<Type, Func<object, CancellationToken, ValueTask>>
        {
            { typeof(CustomerCreatedEvent), async (e, ct) => await HandleCustomerCreatedEvent((CustomerCreatedEvent)e, ct) },
            { typeof(CustomerNameChangedEvent), async (e, ct) => await HandleCustomerEvent((CustomerNameChangedEvent)e, ct) },
            { typeof(CustomerAddressChangedEvent), async (e, ct) => await HandleCustomerEvent((CustomerAddressChangedEvent)e, ct) },
            { typeof(CustomerPhoneNumberChangedEvent), async (e, ct) => await HandleCustomerEvent((CustomerPhoneNumberChangedEvent)e, ct) },
            { typeof(CustomerRoleChangedEvent), async (e, ct) => await HandleCustomerEvent((CustomerRoleChangedEvent)e, ct) },
            { typeof(CustomerProductAddedEvent), async (e, ct) => await HandleCustomerProductAddedEvent((CustomerProductAddedEvent)e, ct) },
            { typeof(CustomerProductRemovedEvent), async (e, ct) => await HandleCustomerProductRemovedEvent((CustomerProductRemovedEvent)e, ct) },
            { typeof(CustomerOrderAddedEvent), async (e, ct) => await HandleCustomerOrderAddedEvent((CustomerOrderAddedEvent)e, ct) },
            { typeof(CustomerOrderRemovedEvent), async (e, ct) => await HandleCustomerOrderRemovedEvent((CustomerOrderRemovedEvent)e, ct) },
            { typeof(CustomerActivatedEvent), async (e, ct) => await HandleCustomerEvent((CustomerActivatedEvent)e, ct) },
            { typeof(CustomerDeactivatedEvent), async (e, ct) => await HandleCustomerEvent((CustomerDeactivatedEvent)e, ct) },
            { typeof(CustomerDeletedEvent), async (e, ct) => await HandleCustomerEvent((CustomerDeletedEvent)e, ct) },
            { typeof(CustomerUndeletedEvent), async (e, ct) => await HandleCustomerEvent((CustomerUndeletedEvent)e, ct) },
            { typeof(ProductCreatedEvent), async (e, ct) => await HandleProductCreatedEvent((ProductCreatedEvent)e, ct) },
            { typeof(ProductBrandChangedEvent), async (e, ct) => await HandleProductEvent((ProductBrandChangedEvent)e, ct) },
            { typeof(ProductModelChangedEvent), async (e, ct) => await HandleProductEvent((ProductModelChangedEvent)e, ct) },
            { typeof(ProductOwnerChangedEvent), async (e, ct) => await HandleProductOwnerChangedEvent((ProductOwnerChangedEvent)e, ct) },
            { typeof(ProductDealerChangedEvent), async (e, ct) => await HandleProductDealerChangedEvent((ProductDealerChangedEvent)e, ct) },
            { typeof(ProductOrderAddedEvent), async (e, ct) => await HandleProductOrderAddedEvent((ProductOrderAddedEvent)e, ct) },
            { typeof(ProductOrderRemovedEvent), async (e, ct) => await HandleProductOrderRemovedEvent((ProductOrderRemovedEvent)e, ct) },
            { typeof(ProductDeviceTypeChangedEvent), async (e, ct) => await HandleProductEvent((ProductDeviceTypeChangedEvent)e, ct) },
            { typeof(ProductPanelChangedEvent), async (e, ct) => await HandleProductEvent((ProductPanelChangedEvent)e, ct) },
            { typeof(ProductWarrantyCardNumberChangedEvent), async (e, ct) => await HandleProductEvent((ProductWarrantyCardNumberChangedEvent)e, ct) },
            { typeof(ProductPurchaseDataChangedEvent), async (e, ct) => await HandleProductEvent((ProductPurchaseDataChangedEvent)e, ct) },
            { typeof(ProductUnrepairableEvent), async (e, ct) => await HandleProductEvent((ProductUnrepairableEvent)e, ct) },
            { typeof(ProductActivatedEvent), async (e, ct) => await HandleProductEvent((ProductActivatedEvent)e, ct) },
            { typeof(ProductDeactivatedEvent), async (e, ct) => await HandleProductEvent((ProductDeactivatedEvent)e, ct) },
            { typeof(ProductDeletedEvent), async (e, ct) => await HandleProductEvent((ProductDeletedEvent)e, ct) },
            { typeof(ProductUndeletedEvent), async (e, ct) => await HandleProductEvent((ProductUndeletedEvent)e, ct) }
        };
    }
    
    public void Apply(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        ApplyAsync(operations, streams, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task ApplyAsync(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken ct = default)
    {
        var eventHandlers = _eventHandlerDefinitions.ToDictionary(
            kvp => kvp.Key,
            kvp => new Func<object, ValueTask>(e => kvp.Value(e, ct))
        );

        var sortedStreams = streams.Where(s => s.AggregateType == typeof(Product))
            .Concat(streams.Where(s => s.AggregateType == typeof(Customer)))
            .Concat(streams).Distinct();

        var otherEvents = new List<object>();
        
        foreach (var stream in sortedStreams)
        {
            var productCreatedEvents = new List<object>();
            var customerCreatedEvents = new List<object>();

            foreach (var @event in stream.Events)
            {
                var eventType = @event.Data.GetType();
                if (eventType == typeof(ProductCreatedEvent))
                {
                    productCreatedEvents.Add(@event.Data);
                }
                else if (eventType == typeof(CustomerCreatedEvent))
                {
                    customerCreatedEvents.Add(@event.Data);
                }
                else
                {
                    otherEvents.Add(@event.Data);
                }
            }
            
            await HandleEvents(productCreatedEvents, eventHandlers);
            await HandleEvents(customerCreatedEvents, eventHandlers);
        }
        
        await HandleEvents(otherEvents, eventHandlers);

        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task HandleEvents(IEnumerable<Object> events, Dictionary<Type, Func<object, ValueTask>> eventHandlers)
    {
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            if (eventHandlers.TryGetValue(eventType, out var handler))
            {
                await handler(@event);
            }
            else
            {
                throw new InvalidOperationException($"No handler found for event type {eventType}");
            }
        }
    }

    private async ValueTask HandleCustomerEvent(CustomerEvent @event, CancellationToken ct = default)
    {
        if (!_cache.TryGetValue<Customer>(@event.CustomerId.Value, out var customer))
        {
            customer = await _dbContext.Customers.AsNoTracking().FirstAsync(p => p.Id == @event.CustomerId, ct);
            _cache.Store(@event.CustomerId.Value, customer);
        }
        customer.Apply(@event);
    }

    private async ValueTask HandleProductEvent(ProductEvent @event, CancellationToken ct = default)
    {
        if (!_cache.TryGetValue<Product>(@event.ProductId.Value, out var product))
        {
            product = await _dbContext.Products.AsNoTracking().FirstAsync(p => p.Id == @event.ProductId, ct);
            _cache.Store(@event.ProductId.Value, product);
        }
        product.Apply(@event);
    }

    private async ValueTask HandleCustomerCreatedEvent(CustomerCreatedEvent @event, CancellationToken ct = default)
    {
        var customer = Customer.Create(@event);
        _dbContext.Customers.Add(customer);
        _cache.Store(customer.Id.Value, customer);

        foreach (var productId in @event.ProductIds)
        {
            if (!_cache.TryGetValue<Product>(productId.Value, out var product))
            {
                product = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId, ct);
                if (product is null) continue;
                _cache.Store(productId.Value, product);
            }

            var customerProduct = new CustomerProductLink
            {
                CustomerId = @event.CustomerId,
                ProductId = productId
            };

            await AddCustomerProduct(customerProduct, ct);
        }

        foreach (var orderId in @event.OrderIds)
        {
            var customerOrder = new CustomerOrderLink
            {
                CustomerId = @event.CustomerId,
                OrderId = orderId,
                CentreId = new CentreId(orderId.CentreId),
            };
            
            await AddCustomerOrder(customerOrder, ct);
        }
    }

    private async ValueTask HandleCustomerProductAddedEvent(CustomerProductAddedEvent @event, CancellationToken ct = default)
    {
        var customerProduct = new CustomerProductLink { CustomerId = @event.CustomerId, ProductId = @event.ProductId };
        await AddCustomerProduct(customerProduct, ct);
        await HandleCustomerEvent(@event, ct);
    }

    private async ValueTask HandleCustomerProductRemovedEvent(CustomerProductRemovedEvent @event, CancellationToken ct = default)
    {
        var customerProduct = new CustomerProductLink { CustomerId = @event.CustomerId, ProductId = @event.ProductId };
        await RemoveCustomerProduct(customerProduct, ct);
        await HandleCustomerEvent(@event, ct);
    }
    
    private async ValueTask HandleCustomerOrderAddedEvent(CustomerOrderAddedEvent @event, CancellationToken ct = default)
    {
        var orderId = @event.OrderId;
        var centreId = new CentreId(@event.OrderId.CentreId);

        var customerOrder = new CustomerOrderLink { CustomerId = @event.CustomerId, OrderId = orderId, CentreId = centreId };
        await AddCustomerOrder(customerOrder, ct);
        await HandleCustomerEvent(@event, ct);
    }

    private async ValueTask HandleCustomerOrderRemovedEvent(CustomerOrderRemovedEvent @event, CancellationToken ct = default)
    {
        var orderId = @event.OrderId;
        var centreId = new CentreId(@event.OrderId.CentreId);
        
        var customerOrder = new CustomerOrderLink { CustomerId = @event.CustomerId, OrderId = orderId, CentreId = centreId };
        await RemoveCustomerOrder(customerOrder, ct);
        await HandleCustomerEvent(@event, ct);
    }

    private async ValueTask HandleProductCreatedEvent(ProductCreatedEvent @event, CancellationToken ct = default)
    {
        var product = Product.Create(@event);
        _dbContext.Products.Add(product);
        _cache.Store(product.Id.Value, product);

        if (product.OwnerId is not null)
        {
            var ownerId = product.OwnerId;
            if (!_cache.TryGetValue<Customer>(ownerId.Value, out var owner))
            {
                owner = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == ownerId, ct);
                if (owner is not null) _cache.Store(ownerId.Value, owner);
            }
            if (owner is not null)
            {
                var customerProduct = new CustomerProductLink
                {
                    CustomerId = ownerId,
                    ProductId = product.Id
                };

                await AddCustomerProduct(customerProduct, ct);
            }
        }
        
        if (product.DealerId is not null)
        {
            var dealerId = product.DealerId;
            if (!_cache.TryGetValue<Customer>(dealerId.Value, out var dealer))
            {
                dealer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dealerId, ct);
                if (dealer is not null) _cache.Store(dealerId.Value, dealer);
            }
            if (dealer is not null)
            {
                _cache.Store(dealerId.Value, dealer);
                var customerProduct = new CustomerProductLink
                {
                    CustomerId = dealerId,
                    ProductId = product.Id
                };

                await AddCustomerProduct(customerProduct, ct);
            }
        }

        foreach (var orderId in @event.OrderIds)
        {
            var productOrder = new ProductOrderLink
            {
                ProductId = product.Id,
                OrderId = orderId,
                CentreId = new CentreId(orderId.CentreId)
            };

            await AddProductOrder(productOrder, ct);
        }
    }

    private async ValueTask HandleProductOwnerChangedEvent(ProductOwnerChangedEvent @event, CancellationToken ct = default)
    {
        if (!_cache.TryGetValue<Product>(@event.ProductId.Value, out var product))
        {
            product = await _dbContext.Products.AsNoTracking().FirstAsync(p => p.Id == @event.ProductId, ct);
            _cache.Store(@event.ProductId.Value, product);
        }
        
        if (product.OwnerId is not null)
        {
            var customerProduct = new CustomerProductLink { ProductId = @event.ProductId, CustomerId = product.OwnerId };
            await RemoveCustomerProduct(customerProduct, ct);
        }

        if (@event.OwnerId is not null)
        {
            var customerProduct = new CustomerProductLink { ProductId = @event.ProductId, CustomerId = @event.OwnerId };
            await AddCustomerProduct(customerProduct, ct);
        }
        
        await HandleProductEvent(@event, ct);
    }
    
    private async ValueTask HandleProductDealerChangedEvent(ProductDealerChangedEvent @event, CancellationToken ct = default)
    {
        if (!_cache.TryGetValue<Product>(@event.ProductId.Value, out var product))
        {
            product = await _dbContext.Products.AsNoTracking().FirstAsync(p => p.Id == @event.ProductId, ct);
            _cache.Store(@event.ProductId.Value, product);
        }
        
        if (product.DealerId is not null)
        {
            var customerProduct = new CustomerProductLink { ProductId = @event.ProductId, CustomerId = product.DealerId };
            await RemoveCustomerProduct(customerProduct, ct);
        }

        if (@event.DealerId is not null)
        {
            var customerProduct = new CustomerProductLink { ProductId = @event.ProductId, CustomerId = @event.DealerId };
            await AddCustomerProduct(customerProduct, ct);
        }
        
        await HandleProductEvent(@event, ct);
    }
    
    private async ValueTask HandleProductOrderAddedEvent(ProductOrderAddedEvent @event, CancellationToken ct = default)
    {
        var orderId = @event.OrderId;
        var centreId = new CentreId(@event.OrderId.CentreId);

        var productOrder = new ProductOrderLink { ProductId = @event.ProductId, OrderId = orderId, CentreId = centreId };
        await AddProductOrder(productOrder, ct);
        await HandleProductEvent(@event, ct);
    }

    private async ValueTask HandleProductOrderRemovedEvent(ProductOrderRemovedEvent @event, CancellationToken ct = default)
    {
        var orderId = @event.OrderId;
        var centreId = new CentreId(@event.OrderId.CentreId);

        var productOrder = new ProductOrderLink { ProductId = @event.ProductId, OrderId = orderId, CentreId = centreId };
        await RemoveProductOrder(productOrder, ct);
        await HandleProductEvent(@event, ct);
    }

    private async ValueTask AddCustomerProduct(CustomerProductLink customerProductLink, CancellationToken ct = default)
    {
        if (_dbContext.CustomerProducts.Local
            .Any(cp => cp.CustomerId == customerProductLink.CustomerId && cp.ProductId == customerProductLink.ProductId))
        {
            return;
        }
        
        if (!await _dbContext.CustomerProducts.AsNoTracking()
                .AnyAsync(cp => cp.CustomerId == customerProductLink.CustomerId && cp.ProductId == customerProductLink.ProductId, ct))
        {
            _dbContext.CustomerProducts.Add(customerProductLink);
        }
    }

    private async ValueTask RemoveCustomerProduct(CustomerProductLink customerProductLink, CancellationToken ct = default)
    {
        var tracked = _dbContext.CustomerProducts.Local
            .FirstOrDefault(cp => cp.CustomerId == customerProductLink.CustomerId && cp.ProductId == customerProductLink.ProductId);
        
        if (tracked is not null)
        {
            _dbContext.CustomerProducts.Remove(tracked);
            return;
        }
        
        if (await _dbContext.CustomerProducts.AsNoTracking()
            .AnyAsync(cp => cp.CustomerId == customerProductLink.CustomerId && cp.ProductId == customerProductLink.ProductId, ct))
        {
            _dbContext.CustomerProducts.Attach(customerProductLink);
            _dbContext.CustomerProducts.Remove(customerProductLink);
        }
    }
    
    private async ValueTask AddCustomerOrder(CustomerOrderLink customerOrder, CancellationToken ct = default)
    {
        if (_dbContext.CustomerOrders.Local
            .Any(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId))
        {
            return;
        }
        
        if (!await _dbContext.CustomerOrders.AsNoTracking()
                .AnyAsync(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId, ct))
        {
            _dbContext.CustomerOrders.Add(customerOrder);
        }
    }
    
    private async ValueTask RemoveCustomerOrder(CustomerOrderLink customerOrder, CancellationToken ct = default)
    {
        var tracked = _dbContext.CustomerOrders.Local
            .FirstOrDefault(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId);
        
        if (tracked is not null)
        {
            _dbContext.CustomerOrders.Remove(tracked);
            return;
        }
        
        if (await _dbContext.CustomerOrders.AsNoTracking()
                .AnyAsync(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId, ct))
        {
            _dbContext.CustomerOrders.Attach(customerOrder);
            _dbContext.CustomerOrders.Remove(customerOrder);
        }
    }
    
    private async ValueTask AddProductOrder(ProductOrderLink productOrder, CancellationToken ct = default)
    {
        if (_dbContext.ProductOrders.Local
            .Any(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId))
        {
            return;
        }
        
        if (!await _dbContext.ProductOrders.AsNoTracking()
                .AnyAsync(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId, ct))
        {
            _dbContext.ProductOrders.Add(productOrder);
        }
    }

    private async ValueTask RemoveProductOrder(ProductOrderLink productOrder, CancellationToken ct = default)
    {
        var tracked = _dbContext.ProductOrders.Local
            .FirstOrDefault(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId);
        
        if (tracked is not null)
        {
            _dbContext.ProductOrders.Remove(tracked);
            return;
        }
        
        if (await _dbContext.ProductOrders.AsNoTracking()
                .AnyAsync(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId, ct))
        {
            _dbContext.ProductOrders.Attach(productOrder);
            _dbContext.ProductOrders.Remove(productOrder);
        }
    }
}