using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.Configurations;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Commerce.Infrastructure.Common.Services;

public class EventSubscriptionManager : IEventSubscriptionManager
{
    private readonly List<(Customer Customer, Action<CustomerEvent> Handler)> _customerSubscriptions = new();
    private readonly List<(Product Product, Action<ProductEvent> Handler)> _productSubscriptions = new();
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _domainEventsKey;
    private Queue<IDomainEvent>? _domainEvents;

    public EventSubscriptionManager(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IOptions<DomainEventsSettings> options)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _domainEventsKey = options.Value.DomainEventsKey;
    }
    
    private void EnqueueDomainEvent(IDomainEvent @event)
    {
        _domainEvents ??= _httpContextAccessor.HttpContext!.Items.TryGetValue(_domainEventsKey, out var value) 
                          && value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new();

        _domainEvents.Enqueue(@event);
        _httpContextAccessor.HttpContext!.Items[_domainEventsKey] = _domainEvents;
    }

    public void SubscribeToCustomerEvents(Customer customer)
    {
        if (_customerSubscriptions.Any(s => s.Customer == customer))
        {
            return;
        }
        
        void CustomerEventHandler(CustomerEvent @event)
        {
            _unitOfWork.CustomerRepository.Append(@event);
            
            if (@event is IDomainEvent domainEvent)
            {
                EnqueueDomainEvent(domainEvent);
            }
        }

        customer.CustomerEventHandler.EventOccurred += CustomerEventHandler;
        _customerSubscriptions.Add((customer, CustomerEventHandler));
    }

    public void UnsubscribeFromCustomerEvents(Customer customer)
    {
        var subscription = _customerSubscriptions.FirstOrDefault(s => s.Customer == customer);
        if (subscription != default)
        {
            customer.CustomerEventHandler.EventOccurred -= subscription.Handler;
            _unitOfWork.CachingService.Store(customer.Id.Value, customer);
            _customerSubscriptions.Remove(subscription);
        }
    }
    
    public void SubscribeToProductEvents(Product product)
    {
        if (_productSubscriptions.Any(s => s.Product == product))
        {
            return;
        }
        
        void ProductEventHandler(ProductEvent @event)
        {
            _unitOfWork.ProductRepository.Append(@event);

            if (@event is IDomainEvent domainEvent)
            {
                EnqueueDomainEvent(domainEvent);
            }
        }

        product.ProductEventHandler.EventOccurred += ProductEventHandler;
        _productSubscriptions.Add((product, ProductEventHandler));
    }

    public void UnsubscribeFromProductEvents(Product product)
    {
        var subscription = _productSubscriptions.FirstOrDefault(s => s.Product == product);
        if (subscription != default)
        {
            product.ProductEventHandler.EventOccurred -= subscription.Handler;
            _unitOfWork.CachingService.Store(product.Id.Value, product);
            _productSubscriptions.Remove(subscription);
        }
    }

    public void Dispose()
    {
        foreach (var (customer, handler) in _customerSubscriptions)
        {
            customer.CustomerEventHandler.EventOccurred -= handler;
            _unitOfWork.CachingService.Store(customer.Id.Value, customer);
        }
        _customerSubscriptions.Clear();
        
        foreach (var (product, handler) in _productSubscriptions)
        {
            product.ProductEventHandler.EventOccurred -= handler;
            _unitOfWork.CachingService.Store(product.Id.Value, product);
        }
        _productSubscriptions.Clear();
    }
}