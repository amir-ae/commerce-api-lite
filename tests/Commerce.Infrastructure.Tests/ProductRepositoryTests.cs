using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Commerce.Fixtures;
using Commerce.Infrastructure.Common.Services;
using Commerce.Infrastructure.Products.Repositories;
using Shouldly;
using Xunit;

namespace Commerce.Infrastructure.Tests;

public class ProductRepositoryTests : CommerceTests
{
    private readonly ProductMartenRepository _sut;

    public ProductRepositoryTests(CommerceApplicationFactory<Program> factory) : base(factory)
    {
        _sut = new ProductMartenRepository(OpenSession(), new CachingService());
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
        result.All(p => p.Customers.Any()).ShouldBeTrue();
    }
    
    [Theory, TestPriority(4)]
    [InlineData("3")]
    public async Task should_not_return_deleted_records(string id)
    {
        var result = await _sut.ListAsync();

        result.Any(x => x.Id == new ProductId(id)).ShouldBeFalse();
    }
    
    [Theory, TestPriority(5)]
    [LoadData("product")]
    public async Task should_return_record_detail_by_order_id(Product product)
    {
        var orderId = product.OrderIds.First();
        
        var result = await _sut.DetailByOrderIdAsync(orderId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(product.Id);
        result.Customers.Any().ShouldBeTrue();
    }
    
    [Theory, TestPriority(6)]
    [LoadData("product")]
    public async Task should_return_records_detail_by_centre_id(Product product)
    {
        var centreId = new CentreId(product.OrderIds.First().CentreId);
        
        var result = await _sut.DetailByCentreIdAsync(centreId);

        result.ShouldNotBeEmpty();
        result.Select(p => p.Id).ShouldContain(product.Id);
        result.ForEach(p => p.OrderIds.Select(o => o.CentreId).ShouldContain(centreId.Value));
        result.All(p => p.Customers.Any()).ShouldBeTrue();
    }
    
    [Theory, TestPriority(7)]
    [LoadData("product.Id")]
    public async Task should_return_record_by_id(ProductId id)
    {
        var result = await _sut.ByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }

    [Theory, TestPriority(8)]
    [LoadData("product.Id")]
    public async Task should_return_record_detail_by_id(ProductId id)
    {
        var result = await _sut.DetailByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
        result.Customers.Any().ShouldBeTrue();
    }

    [Fact, TestPriority(9)]
    public async Task should_return_null_with_id_not_present()
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var result = await _sut.ByIdAsync(productId);

        result.ShouldBeNull();
    }

    [Theory, TestPriority(10)]
    [LoadData("product.Id")]
    public async Task should_return_true_on_check_by_id(ProductId id)
    {
        var result = await _sut.CheckByIdAsync(id);
        result.ShouldBeTrue();
    }
    
    [Fact, TestPriority(11)]
    public async Task should_return_false_on_check_by_id_not_present()
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var result = await _sut.CheckByIdAsync(productId);
        result.ShouldBeFalse();
    }

    [Theory, TestPriority(12)]
    [LoadData("product")]
    public async Task should_add_new_product(Product product)
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var productCreatedEvent = new ProductCreatedEvent(
            productId,
            product.Model,
            product.Brand,
            null, null, null,null, null, null, 
            null, null, null, null, null, null, 
            null, null,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Create(productCreatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(productId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId);
    }
    
    [Theory, TestPriority(13)]
    [LoadData("product")]
    public async Task should_update_a_product(Product product)
    {
        var productId = new ProductId("PC");
        var productBrandChangedEvent = new ProductBrandChangedEvent(
            productId,
            product.Brand,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(productBrandChangedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(productId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId);
        result.Brand.ShouldBe(product.Brand);
    }
    
    [Theory, TestPriority(14)]
    [LoadData("product")]
    public async Task should_return_record_events_by_id(Product product)
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var productCreatedEvent = new ProductCreatedEvent(
            productId,
            product.Model,
            product.Brand,
            null, null, null, null,null, null, 
            null, null, null,null, null, null, 
            null, null, new AppUserId(Guid.NewGuid())
        );
        var productOwnerChangedEvent = new ProductOwnerChangedEvent(
            productId,
            product.OwnerId,
            null,
            false,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Create(productCreatedEvent);
        _sut.Append(productOwnerChangedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.EventsByIdAsync(productId);

        result.ShouldNotBeNull();
        result.ProductCreatedEvent.ShouldNotBeNull();
        result.ProductOwnerChangedEvents.Last().OwnerId.ShouldBe(product.OwnerId);
    }
    
    [Theory, TestPriority(15)]
    [InlineData("PC")]
    public async Task should_activate_product(string id)
    {
        var productId = new ProductId(id);
        var productActivatedEvent = new ProductActivatedEvent(
            productId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(productActivatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(productId);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }
    
    [Theory, TestPriority(16)]
    [InlineData("PC")]
    public async Task should_deactivate_product(string id)
    {
        var productId = new ProductId(id);
        var productDeactivatedEvent = new ProductDeactivatedEvent(
            productId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(productDeactivatedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(productId);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }
    
    [Theory, TestPriority(17)]
    [InlineData("PB")]
    public async Task should_delete_product(string id)
    {
        var productId = new ProductId(id);
        var productDeletedEvent = new ProductDeletedEvent(
            productId,
            new AppUserId(Guid.NewGuid())
        );
        
        _sut.Append(productDeletedEvent);
        await _sut.SaveChangesAsync();
        var result = await _sut.ByIdAsync(productId);
        
        result.ShouldNotBeNull();
        result.IsDeleted.ShouldBe(true);
    }
}