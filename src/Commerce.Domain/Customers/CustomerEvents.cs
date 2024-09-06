using System.Diagnostics.CodeAnalysis;
using Commerce.Domain.Customers.Events;

namespace Commerce.Domain.Customers;

public sealed record CustomerEvents
{
    [SetsRequiredMembers]
    public CustomerEvents(
        CustomerCreatedEvent customerCreatedEvent,
        List<CustomerNameChangedEvent>? customerNameChangedEvents = null,
        List<CustomerAddressChangedEvent>? customerAddressChangedEvents = null,
        List<CustomerPhoneNumberChangedEvent>? customerPhoneNumberChangedEvents = null,
        List<CustomerRoleChangedEvent>? customerRoleChangedEvents = null,
        List<CustomerProductAddedEvent>? customerProductAddedEvents = null,
        List<CustomerProductRemovedEvent>? customerProductRemovedEvents = null,
        List<CustomerOrderAddedEvent>? customerOrderAddedEvents = null,
        List<CustomerOrderRemovedEvent>? customerOrderRemovedEvents = null,
        List<CustomerActivatedEvent>? customerActivatedEvents = null,
        List<CustomerDeactivatedEvent>? customerDeactivatedEvents = null,
        List<CustomerDeletedEvent>? customerDeletedEvents = null,
        List<CustomerUndeletedEvent>? customerUndeletedEvents = null)
    {
        CustomerCreatedEvent = customerCreatedEvent;
        CustomerNameChangedEvents = customerNameChangedEvents ?? new();
        CustomerAddressChangedEvents = customerAddressChangedEvents ?? new();
        CustomerPhoneNumberChangedEvents = customerPhoneNumberChangedEvents ?? new();
        CustomerRoleChangedEvents = customerRoleChangedEvents ?? new();
        CustomerProductAddedEvents = customerProductAddedEvents ?? new();
        CustomerProductRemovedEvents = customerProductRemovedEvents ?? new();
        CustomerOrderAddedEvents = customerOrderAddedEvents ?? new();
        CustomerOrderRemovedEvents = customerOrderRemovedEvents ?? new();
        CustomerActivatedEvents = customerActivatedEvents ?? new();
        CustomerDeactivatedEvents = customerDeactivatedEvents ?? new();
        CustomerDeletedEvents = customerDeletedEvents ?? new();
        CustomerUndeletedEvents = customerUndeletedEvents ?? new();
    }
    
    public required CustomerCreatedEvent CustomerCreatedEvent { get; init; }
    public List<CustomerNameChangedEvent> CustomerNameChangedEvents { get; set; }
    public List<CustomerAddressChangedEvent> CustomerAddressChangedEvents { get; set; }
    public List<CustomerPhoneNumberChangedEvent> CustomerPhoneNumberChangedEvents { get; set; }
    public List<CustomerRoleChangedEvent> CustomerRoleChangedEvents { get; init; }
    public List<CustomerProductAddedEvent> CustomerProductAddedEvents { get; set; }
    public List<CustomerProductRemovedEvent> CustomerProductRemovedEvents { get; set; }
    public List<CustomerOrderAddedEvent> CustomerOrderAddedEvents { get; set; }
    public List<CustomerOrderRemovedEvent> CustomerOrderRemovedEvents { get; set; }
    public List<CustomerActivatedEvent> CustomerActivatedEvents { get; set; }
    public List<CustomerDeactivatedEvent> CustomerDeactivatedEvents { get; set; }
    public List<CustomerDeletedEvent> CustomerDeletedEvents { get; set; }
    public List<CustomerUndeletedEvent> CustomerUndeletedEvents { get; set; }
}