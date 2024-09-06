using System.Runtime.Serialization;
using Commerce.Domain.Common.Models;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products;

namespace Commerce.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId, string>
{
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public CityId CityId { get; private set; }
    public string Address { get; private set; }
    public CustomerRole Role { get; private set; }
    public HashSet<ProductId> ProductIds { get; private set; } = new();
    [IgnoreDataMember]
    public HashSet<CustomerProductLink> Products { get; private set; } = new();
    public HashSet<OrderId> OrderIds { get; private set; } = new();
    [IgnoreDataMember]
    public HashSet<CustomerOrderLink> Orders { get; private set; } = new();

    public CustomerEventHandler<CustomerEvent> CustomerEventHandler { get; } = new();
    
    private Customer(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole role,
        HashSet<ProductId> productIds,
        HashSet<OrderId> orderIds,
        AppUserId createdBy,
        DateTimeOffset createdAt)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        ProductIds = productIds;
        OrderIds = orderIds;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        IsActive = true;
        IsDeleted = false;
    }

    public static Customer Create(CustomerCreatedEvent created) =>
        new (created.CustomerId,
            created.FirstName,
            created.MiddleName,
            created.LastName,
            created.FullName,
            created.PhoneNumber,
            created.CityId,
            created.Address,
            created.Role,
            created.ProductIds,
            created.OrderIds,
            created.Actor,
            created.CreatedAt);

    public static Customer Construct(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<OrderId>? orderIds,
        AppUserId createdBy,
        DateTimeOffset? createdAt,
        Func<CustomerCreatedEvent, Customer> create)
    {
        var @event = new CustomerCreatedEvent(
            id,
            firstName,
            middleName,
            lastName,
            fullName,
            phoneNumber,
            cityId,
            address,
            role,
            productIds,
            orderIds,
            createdBy,
            createdAt);

        return create(@event);
    }

    public void UpdateName(string? firstName, string? middleName, string? lastName, AppUserId updateBy, 
        DateTimeOffset? updateAt)
    {
        var shouldUpdate = !string.IsNullOrWhiteSpace(firstName) && firstName != FirstName
                       || middleName is not null && middleName != MiddleName
                       || !string.IsNullOrWhiteSpace(lastName) && lastName != LastName;
        if (!shouldUpdate) return;
        
        var @event = new CustomerNameChangedEvent(
            Id,
            firstName ?? FirstName,
            middleName ?? MiddleName,
            lastName ?? LastName,
            null,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void UpdatePhoneNumber(PhoneNumber? phoneNumber, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = phoneNumber is not null && phoneNumber != PhoneNumber;
        if (!shouldUpdate) return;

        var @event = new CustomerPhoneNumberChangedEvent(
            Id,
            phoneNumber!,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateAddress(CityId? cityId, string? address, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = cityId is not null && cityId != CityId
                       || !string.IsNullOrWhiteSpace(address) && address != Address;
        if (!shouldUpdate) return;
        
        var @event = new CustomerAddressChangedEvent(
            Id,
            cityId ?? CityId,
            address ?? Address,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateRole(CustomerRole? role, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = role is not null && role != Role;
        if (!shouldUpdate) return;
        
        var @event = new CustomerRoleChangedEvent(
            Id,
            role!,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void AddProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !ProductIds.Contains(productId);
        if (!shouldUpdate) return;
        
        var productIds = new HashSet<ProductId>(ProductIds) { productId };
        
        var @event = new CustomerProductAddedEvent(
            Id,
            productId,
            productIds,
            Role,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void RemoveProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = ProductIds.Contains(productId);
        if (!shouldUpdate) return;
        
        var productIds = new HashSet<ProductId>(ProductIds);
        productIds.Remove(productId);
        
        var @event = new CustomerProductRemovedEvent(
            Id,
            productId,
            productIds,
            Role,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void AddProducts(HashSet<ProductId>? productIds, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = productIds is not null && !productIds.SetEquals(ProductIds);
        if (!shouldUpdate) return;
        
        var productIdsToAdd = productIds!.Except(ProductIds);
        foreach (var productId in productIdsToAdd)
        {
            AddProduct(productId, updateBy, updateAt);
        }
    }
    
    public void RemoveProducts(HashSet<ProductId>? productIds, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = productIds is not null && !productIds.SetEquals(ProductIds);
        if (!shouldUpdate) return;
        
        var productIdsToRemove = ProductIds.Except(productIds!);
        foreach (var productId in productIdsToRemove)
        {
            RemoveProduct(productId, updateBy, updateAt);
        }
    }
    
    public void AddOrder(OrderId orderId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !OrderIds.Contains(orderId);
        if (!shouldUpdate) return;
        
        var orderIds = new HashSet<OrderId>(OrderIds) { orderId };
        
        var @event = new CustomerOrderAddedEvent(
            Id,
            orderId,
            orderIds,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void RemoveOrder(OrderId orderId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = OrderIds.Contains(orderId);
        if (!shouldUpdate) return;
        
        var orderIds = new HashSet<OrderId>(OrderIds);
        orderIds.Remove(orderId);
        
        var @event = new CustomerOrderRemovedEvent(
            Id,
            orderId,
            orderIds,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void AddOrders(HashSet<OrderId>? orderIds, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orderIds is not null && !orderIds.SetEquals(OrderIds);
        if (!shouldUpdate) return;
        
        var ordersToAdd = orderIds!.Except(OrderIds);
        foreach (var orderId in ordersToAdd)
        {
            AddOrder(orderId, updateBy, updateAt);
        }
    }
    
    public void RemoveOrders(HashSet<OrderId>? orderIds, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orderIds is not null && !orderIds.SetEquals(OrderIds);
        if (!shouldUpdate) return;
        
        var ordersToRemove = OrderIds.Except(orderIds!);
        foreach (var orderId in ordersToRemove)
        {
            RemoveOrder(orderId, updateBy, updateAt);
        }
    }

    public void Activate(AppUserId activateBy)
    {
        if (IsActive) return;
        
        var @event = new CustomerActivatedEvent(Id, activateBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Deactivate(AppUserId deactivateBy)
    {
        if (!IsActive) return;
        
        var @event = new CustomerDeactivatedEvent(Id, deactivateBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Delete(AppUserId deleteBy)
    {
        if (IsDeleted) return;
        
        var @event = new CustomerDeletedEvent(Id, deleteBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Undelete(AppUserId undeleteBy)
    {
        if (!IsDeleted) return;
        
        var @event = new CustomerUndeletedEvent(Id, undeleteBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void SetProductIds(HashSet<ProductId> productIds)
    {
        if (productIds.SetEquals(ProductIds)) return;
        ProductIds = productIds;
    }
    
    public void AddProducts(HashSet<Product>? products)
    {
        if (products is null || Products.Select(cp => cp.Product).ToHashSet().SetEquals(products)) return;
        Products = products
            .Select(p => new CustomerProductLink { ProductId = p.Id, Product = p, CustomerId = Id })
            .ToHashSet();
    }
    
    public void SetOrderIds(HashSet<OrderId> orderIds)
    {
        if (orderIds.SetEquals(OrderIds)) return;
        OrderIds = orderIds;
    }

    public void Apply(CustomerNameChangedEvent changed)
    {
        FirstName = changed.FirstName;
        MiddleName = changed.MiddleName;
        LastName = changed.LastName;
        FullName = changed.FullName;
        LastModifiedAt = changed.NameChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(CustomerPhoneNumberChangedEvent changed)
    {
        PhoneNumber = changed.PhoneNumber;
        LastModifiedAt = changed.PhoneNumberChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(CustomerAddressChangedEvent changed)
    {
        CityId = changed.CityId;
        Address = changed.Address;
        LastModifiedAt = changed.AddressChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(CustomerRoleChangedEvent changed)
    {
        Role = changed.Role;
        LastModifiedAt = changed.RoleChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(CustomerProductAddedEvent added)
    {
        ProductIds.Add(added.ProductId);
        LastModifiedAt = added.ProductAddedAt;
        LastModifiedBy = added.Actor;
        Version++;
    }

    public void Apply(CustomerProductRemovedEvent removed)
    {
        ProductIds.Remove(removed.ProductId);
        LastModifiedAt = removed.ProductRemovedAt;
        LastModifiedBy = removed.Actor;
        Version++;
    }
    
    public void Apply(CustomerOrderAddedEvent added)
    {
        OrderIds.Add(added.OrderId);
        LastModifiedAt = added.OrderAddedAt;
        LastModifiedBy = added.Actor;
        Version++;
    }

    public void Apply(CustomerOrderRemovedEvent removed)
    {
        OrderIds.Remove(removed.OrderId);
        LastModifiedAt = removed.OrderRemovedAt;
        LastModifiedBy = removed.Actor;
        Version++;
    }

    public void Apply(CustomerActivatedEvent activated)
    {
        IsActive = true;
        LastModifiedAt = activated.ActivatedAt;
        LastModifiedBy = activated.Actor;
        Version++;
    }

    public void Apply(CustomerDeactivatedEvent deactivated)
    {
        IsActive = false;
        LastModifiedAt = deactivated.DeactivatedAt;
        LastModifiedBy = deactivated.Actor;
        Version++;
    }

    public void Apply(CustomerDeletedEvent deleted)
    {
        IsActive = false;
        IsDeleted = true;
        LastModifiedAt = deleted.DeletedAt;
        LastModifiedBy = deleted.Actor;
        Version++;
    }

    public void Apply(CustomerUndeletedEvent undeleted)
    {
        IsActive = true;
        IsDeleted = false;
        LastModifiedAt = undeleted.UndeletedAt;
        LastModifiedBy = undeleted.Actor;
        Version++;
    }
    
    public void Apply(CustomerEvent @event)
    {
        switch (@event)
        {
            case CustomerNameChangedEvent nameChangedEvent:
                Apply(nameChangedEvent);
                break;
            case CustomerAddressChangedEvent addressChangedEvent:
                Apply(addressChangedEvent);
                break;
            case CustomerPhoneNumberChangedEvent phoneNumberChangedEvent:
                Apply(phoneNumberChangedEvent);
                break;
            case CustomerRoleChangedEvent roleChangedEvent:
                Apply(roleChangedEvent);
                break;
            case CustomerProductAddedEvent productAddedEvent:
                Apply(productAddedEvent);
                break;
            case CustomerProductRemovedEvent productRemovedEvent:
                Apply(productRemovedEvent);
                break;
            case CustomerOrderAddedEvent orderAddedEvent:
                Apply(orderAddedEvent);
                break;
            case CustomerOrderRemovedEvent orderRemovedEvent:
                Apply(orderRemovedEvent);
                break;
            case CustomerActivatedEvent activatedEvent:
                Apply(activatedEvent);
                break;
            case CustomerDeactivatedEvent deactivatedEvent:
                Apply(deactivatedEvent);
                break;
            case CustomerDeletedEvent deletedEvent:
                Apply(deletedEvent);
                break;
            case CustomerUndeletedEvent undeletedEvent:
                Apply(undeletedEvent);
                break;
            default:
                throw new InvalidOperationException($"Unhandled customer event type {@event.GetType()}");
        }
    }

#pragma warning disable CS8618
    public Customer() { }
#pragma warning restore CS8618
}