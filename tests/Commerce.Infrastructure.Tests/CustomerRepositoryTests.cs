using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Fixtures;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Services;
using Commerce.Infrastructure.Customers.Repositories;
using Marten;
using Moq;
using Shouldly;
using Xunit;

namespace Commerce.Infrastructure.Tests;

public class CustomerRepositoryTests : CommerceTests
{
    private readonly CustomerMartenRepository _sut;

    public CustomerRepositoryTests(CommerceApplicationFactory<Program> factory) : base(factory)
    {
        _sut = new CustomerMartenRepository(OpenSession(), new CachingService());
    }

    [Fact, TestPriority(1)]
    public async Task should_get_data()
    {
        var result = await _sut.ListAsync();

        result.ShouldNotBeNull();
    }
    
    [Theory, TestPriority(2)]
    [InlineData(2)]
    public async Task should_get_all_records(int count)
    {
        var result = await _sut.ListAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
    }
    
    [Theory, TestPriority(3)]
    [InlineData(2)]
    public async Task should_get_all_records_in_detail(int count)
    {
        var result = await _sut.ListDetailAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
        result.All(c => c.Products.Any()).ShouldBeTrue();
    }
    
    [Theory, TestPriority(4)]
    [InlineData("3")]
    public async Task should_not_return_deleted_records(string id)
    {
        var result = await _sut.ListAsync();

        result.Any(x => x.Id == new CustomerId(id)).ShouldBeFalse();
    }
    
    [Theory, TestPriority(5)]
    [InlineData("1", "2", "3")]
    public async Task should_return_records_detail_by_ids(string id1, string id2, string id3)
    {
        var ids = new List<string> { id1, id2, id3 };
        var customerIds = ids.Select(id => new CustomerId(id)).ToList();
        
        var result = await _sut.DetailByIdsAsync(customerIds);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
    }

    [Theory, TestPriority(6)]
    [LoadData("customer.Id")]
    public async Task should_return_record_by_id(CustomerId id)
    {
        var result = await _sut.ByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }
    
    [Theory, TestPriority(7)]
    [LoadData("customer.Id")]
    public async Task should_return_record_detail_by_id(CustomerId id)
    {
        var result = await _sut.DetailByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
        result.Products.Any().ShouldBeTrue();
    }

    [Theory, TestPriority(8)]
    [LoadData("customer")]
    public async Task should_return_record_by_data(Customer customer)
    {
        var result = await _sut.ByDataAsync(customer.FullName, customer.PhoneNumber.Value);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customer.Id);
    }
    
    [Fact, TestPriority(9)]
    public async Task should_return_null_with_id_not_present()
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldBeNull();
    }
    
    [Theory, TestPriority(10)]
    [LoadData("customer.Id")]
    public async Task should_return_true_on_check_by_id(CustomerId id)
    {
        var result = await _sut.CheckByIdAsync(id);
        result.ShouldBeTrue();
    }
    
    [Fact, TestPriority(11)]
    public async Task should_return_false_on_check_by_id_not_present()
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var result = await _sut.CheckByIdAsync(customerId);
        result.ShouldBeFalse();
    }
    
    [Theory, TestPriority(12)]
    [LoadData("customer")]
    public async Task should_add_new_customer(Customer customer)
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var customerCreatedEvent = new CustomerCreatedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            customer.PhoneNumber,
            customer.CityId,
            customer.Address,
            null,
            customer.ProductIds,
            null,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Create(customerCreatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customerId);
    }
    
    [Theory, TestPriority(13)]
    [LoadData("customer")]
    public async Task should_update_a_customer(Customer customer)
    {
        var customerId = new CustomerId("3");
        var customerNameChangedEvent = new CustomerNameChangedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(customerNameChangedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customerId);
        result.FirstName.ShouldBe(customer.FirstName);
        result.MiddleName.ShouldBe(customer.MiddleName);
        result.LastName.ShouldBe(customer.LastName);
    }
    
    [Theory, TestPriority(14)]
    [LoadData("customer")]
    public async Task should_return_record_events_by_id(Customer customer)
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var customerCreatedEvent = new CustomerCreatedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            customer.PhoneNumber,
            customer.CityId,
            customer.Address,
            null,
            customer.ProductIds,
            null,
            new AppUserId(Guid.NewGuid())
        );
        var customerRoleChangedEvent = new CustomerRoleChangedEvent(
            customerId,
            CustomerRole.Dealer,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Create(customerCreatedEvent);
        _sut.Append(customerRoleChangedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.EventsByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.CustomerCreatedEvent.ShouldNotBeNull();
        result.CustomerRoleChangedEvents.Last().Role.ShouldBe(CustomerRole.Dealer);
    }
    
    [Theory, TestPriority(15)]
    [InlineData("3")]
    public async Task should_activate_customer(string id)
    {
        var customerId = new CustomerId(id);
        var customerActivatedEvent = new CustomerActivatedEvent(
            customerId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(customerActivatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(customerId);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }
    
    [Theory, TestPriority(16)]
    [InlineData("3")]
    public async Task should_deactivate_customer(string id)
    {
        var customerId = new CustomerId(id);
        var customerDeactivatedEvent = new CustomerDeactivatedEvent(
            customerId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(customerDeactivatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(customerId);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }
    
    [Theory, TestPriority(17)]
    [InlineData("2")]
    public async Task should_delete_customer(string id)
    {
        var customerId = new CustomerId(id);
        var customerDeletedEvent = new CustomerDeletedEvent(
            customerId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(customerDeletedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(customerId);
        
        result.ShouldNotBeNull();
        result.IsDeleted.ShouldBe(true);
    }
}