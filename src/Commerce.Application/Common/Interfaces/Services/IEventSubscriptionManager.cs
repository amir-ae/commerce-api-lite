using Commerce.Domain.Customers;
using Commerce.Domain.Products;

namespace Commerce.Application.Common.Interfaces.Services;

public interface IEventSubscriptionManager : IDisposable
{
    void SubscribeToCustomerEvents(Customer customer);
    void UnsubscribeFromCustomerEvents(Customer customer);
    void SubscribeToProductEvents(Product product);
    void UnsubscribeFromProductEvents(Product product);
}