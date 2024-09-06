using Commerce.API.Contract.V1.Customers.Responses.Events;

namespace Commerce.API.Contract.V1.Customers.Responses;

public record CustomerEventsResponse(CustomerCreated CustomerCreatedEvent,
    IList<CustomerNameChanged> CustomerNameChangedEvents,
    IList<CustomerAddressChanged> CustomerAddressChangedEvents,
    IList<CustomerPhoneNumberChanged> CustomerPhoneNumberChangedEvents,
    IList<CustomerRoleChanged> CustomerRoleChangedEvents,
    IList<CustomerProductAdded> CustomerProductAddedEvents,
    IList<CustomerProductRemoved> CustomerProductRemovedEvents,
    IList<CustomerOrderAdded> CustomerOrderAddedEvents,
    IList<CustomerOrderRemoved> CustomerOrderRemovedEvents,
    IList<CustomerActivated> CustomerActivatedEvents,
    IList<CustomerDeactivated> CustomerDeactivatedEvents,
    IList<CustomerDeleted> CustomerDeletedEvents,
    IList<CustomerUndeleted> CustomerUndeletedEvents);