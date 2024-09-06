using System.Text.Json;
using Catalog.API.Client;
using Catalog.API.Contract.V1.Cities.Responses;
using Catalog.API.Contract.V1.Serials.Responses;
using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.API.Contract.V1.Customers.Responses.Events;
using Commerce.API.Contract.V1.Customers.Responses.Models;
using Commerce.API.Contract.V1.Products.Responses;
using Commerce.API.Contract.V1.Products.Responses.Events;
using Commerce.API.Contract.V1.Products.Responses.Models;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using Logistics.API.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using CustomerRoleEnum = Commerce.API.Contract.V1.Common.Models.CustomerRole;
using CustomerAggregate = Commerce.Domain.Customers.Customer;

namespace Commerce.Infrastructure.Common.Services;

public class EnrichmentService : IEnrichmentService
{
    private readonly ICatalogClient _catalogClient;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IServiceProvider _serviceProvider;

    public EnrichmentService(ICatalogClient catalogClient, ILogisticsClient logisticsClient, IDistributedCache cache, IServiceProvider serviceProvider)
    {
        _catalogClient = catalogClient;
        _cache = cache;
        _serviceProvider = serviceProvider;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async ValueTask<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.Id, ct);
        return Map(customer, city);
    }

    public async ValueTask<CustomerBriefResponse> EnrichCustomerForListingResponse(CustomerBriefResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.Id, ct);
        return Map(customer, city);
    }

    public async ValueTask<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default)
    {
        if (product.Owner is not null)
        {
            product = product with { Owner = await EnrichCustomerForListingResponse(product.Owner, ct) };
        }
        if (product.Dealer is not null)
        {
            product = product with { Dealer = await EnrichCustomerForListingResponse(product.Dealer, ct) };
        }
        
        if (product.SerialId.HasValue)
        {
            var serial = await Serial(product.SerialId.Value, ct);
            return Map(product, serial);
        }

        return product;
    }
    
    public async ValueTask<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default)
    {
        return customerEvents with
        {
            CustomerCreatedEvent = await EnrichCustomerCreatedEvent(customerEvents.CustomerCreatedEvent, ct),
            CustomerAddressChangedEvents = await EnrichCustomerAddressChangedEvents(customerEvents.CustomerAddressChangedEvents, ct)
        };
    }

    public async ValueTask<CustomerCreated> EnrichCustomerCreatedEvent(CustomerCreated customerCreatedEvent, CancellationToken ct = default)
    {
        var city = await City(customerCreatedEvent.City.Id, ct);
        return Map(customerCreatedEvent, city);
    }
    
    public async ValueTask<IList<CustomerAddressChanged>> EnrichCustomerAddressChangedEvents(
        IList<CustomerAddressChanged> customerAddressChangedEvents, CancellationToken ct = default)
    {
        if (!customerAddressChangedEvents.Any()) return customerAddressChangedEvents;
        
        var tasks = customerAddressChangedEvents
            .Select(async @event => await Map(@event, City(@event.City.Id, ct)));
        
        return (await Task.WhenAll(tasks)).ToList();
    }

    private async ValueTask<CityResponse?> City(int cityId, CancellationToken ct = default)
    {
        var cacheKey = $"city_{cityId}";

        var cachedCity = await _cache.GetStringAsync(cacheKey, token: ct);
        if (cachedCity is not null)
        {
            return JsonSerializer.Deserialize<CityResponse>(cachedCity, _serializerOptions);
        }
        
        var city = await FetchCity(cityId, ct);

        if (city is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(city, _serializerOptions), cacheOptions, token: ct);
            return city;
        }

        return null;
    }

    private async Task<CityResponse?> FetchCity(int cityId, CancellationToken ct = default)
    {
        try
        {
            var result = await _catalogClient.City.ById(cityId, ct);
            if (!result.IsError)
            {
                return result.Value;
            }
        }
        catch
        {
            //ignore
        }
        
        return null;
    }
    
    private async ValueTask<SerialResponse?> Serial(int serialId, CancellationToken ct = default)
    {
        var cacheKey = $"serial_{serialId}";

        var cachedSerial = await _cache.GetStringAsync(cacheKey, token: ct);
        if (cachedSerial is not null)
        {
            return JsonSerializer.Deserialize<SerialResponse>(cachedSerial, _serializerOptions);
        }
        
        var serial = await FetchSerial(serialId, ct);

        if (serial is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(serial, _serializerOptions), cacheOptions, token: ct);
            return serial;
        }
        
        return null;
    }

    private async Task<SerialResponse?> FetchSerial(int serialId, CancellationToken ct = default)
    {
        try
        {
            var result = await _catalogClient.Serial.ById(serialId, ct);
            if (!result.IsError)
            {
                return result.Value;
            }
        }
        catch
        {
            //ignore
        }
        
        return null;
    }

    private static CustomerResponse Map(CustomerResponse customer, CityResponse? city)
    {
        if (city is null) return customer;
        return customer with
        {
            City = new City(
                customer.City.Id,
                city.Name,
                city.Oblast,
                city.PostalCode,
                city.PhoneCode)
        };
    }
    
    private static CustomerBriefResponse Map(CustomerBriefResponse customer, CityResponse? city)
    {
        if (city is null) return customer;
        return customer with
        {
            City = new City(
                customer.City.Id,
                city.Name,
                city.Oblast,
                city.PostalCode,
                city.PhoneCode)
        };
    }
    
    private static Customer Map(Customer customer, CityResponse? city)
    {
        if (city is null || customer.City is null) return customer;
        return customer with
        {
            City = new City(
                customer.City.Id,
                city.Name,
                city.Oblast,
                city.PostalCode,
                city.PhoneCode)
        };
    }
    
    private static Customer Map(Customer customer, CustomerAggregate? customerAggregate)
    {
        if (customerAggregate is null) return customer;
        return customer with
        {
            Name = customerAggregate.FullName,
            Role = (CustomerRoleEnum)customerAggregate.Role.Value,
            FirstName = customerAggregate.FirstName,
            MiddleName = customerAggregate.MiddleName,
            LastName = customerAggregate.LastName,
            PhoneNumber = new PhoneNumber(customerAggregate.PhoneNumber.Value, customerAggregate.PhoneNumber.CountryId.Value, 
                customerAggregate.PhoneNumber.CountryCode, customerAggregate.PhoneNumber.Description),
            City = new City(customerAggregate.CityId.Value),
            Address = customerAggregate.Address
        };
    }
    
    private static CustomerCreated Map(CustomerCreated @event, CityResponse? city)
    {
        if (city is null) return @event;
        return @event with
        {
            City = new City(
                @event.City.Id,
                city.Name,
                city.Oblast,
                city.PostalCode,
                city.PhoneCode)
        };
    }

    private static async ValueTask<CustomerAddressChanged> Map(CustomerAddressChanged @event, ValueTask<CityResponse?> cityTask)
    {
        var city = await cityTask;
        if (city is null) return @event;
        return @event with
        {
            City = new City(
                @event.City.Id,
                city.Name,
                city.Oblast,
                city.PostalCode,
                city.PhoneCode)
        };
    }
    
    private static ProductResponse Map(ProductResponse product, SerialResponse? serial)
    {
        if (serial is null || product.SerialId is null) return product;
        return product with
        {
            Lot = serial.Lot,
            ProductionDate = serial.ProductionDate
        };
    }

    public async Task<Customer> EnrichCustomer(Customer customer, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(customer.Name))
        {
            var customerId = new CustomerId(customer.Id);
            var scope = _serviceProvider.CreateScope();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var customerAggregate = await customerRepository.ByIdAsync(customerId, ct);
            customer = Map(customer, customerAggregate);
        } 
        
        if (customer.City is null) return customer;
        var city = await City(customer.City.Id, ct);
        return Map(customer, city);
    }

    public async ValueTask<ProductEventsResponse> EnrichProductEvents(ProductEventsResponse productEvents, CancellationToken ct = default)
    {
        var productCreatedTask = Task.Run(async () => await EnrichProductCreatedEvent(productEvents.ProductCreatedEvent, ct), ct);
        var ownerChangedTask = Task.Run(async () => await EnrichProductOwnerChangedEvents(productEvents.ProductOwnerChangedEvents, ct), ct);
        var dealerChangedTask = Task.Run(async () => await EnrichProductDealerChangedEvents(productEvents.ProductDealerChangedEvents, ct), ct);

        await Task.WhenAll(productCreatedTask, ownerChangedTask, dealerChangedTask);
            
        return productEvents with
        {
            ProductCreatedEvent = await productCreatedTask,
            ProductOwnerChangedEvents = await ownerChangedTask,
            ProductDealerChangedEvents = await dealerChangedTask
        };
    }
    
    private async Task<ProductCreated> EnrichProductCreatedEvent(ProductCreated @event, CancellationToken ct)
        => @event with
        {
            Owner = @event.Owner is null ? null : await EnrichCustomer(@event.Owner, ct),
            Dealer = @event.Dealer is null ? null : await EnrichCustomer(@event.Dealer, ct)
        };
    
    private async Task<ProductOwnerChanged> EnrichProductOwnerChangedEvent(ProductOwnerChanged @event, CancellationToken ct)
        => @event with { Owner = @event.Owner is null ? null : await EnrichCustomer(@event.Owner, ct) };

    private async Task<ProductDealerChanged> EnrichProductDealerChangedEvent(ProductDealerChanged @event, CancellationToken ct)
        => @event with { Dealer = @event.Dealer is null ? null : await EnrichCustomer(@event.Dealer, ct) };
    
    private async Task<IList<ProductOwnerChanged>> EnrichProductOwnerChangedEvents(
        IList<ProductOwnerChanged> productOwnerChangedEvents, CancellationToken ct = default)
    {
        if (!productOwnerChangedEvents.Any()) return productOwnerChangedEvents;
        
        var tasks = new Task<ProductOwnerChanged>[productOwnerChangedEvents.Count];
        
        for (int i = 0; i < productOwnerChangedEvents.Count; i++)
        {
            var currentEvent = productOwnerChangedEvents[i];
            tasks[i] = EnrichProductOwnerChangedEvent(currentEvent, ct);
        }

        return await Task.WhenAll(tasks);
    }

    private async Task<IList<ProductDealerChanged>> EnrichProductDealerChangedEvents(
        IList<ProductDealerChanged> productDealerChangedEvents, CancellationToken ct = default)
    {
        if (!productDealerChangedEvents.Any()) return productDealerChangedEvents;
        
        var tasks = new Task<ProductDealerChanged>[productDealerChangedEvents.Count];
        
        for (int i = 0; i < productDealerChangedEvents.Count; i++)
        {
            var currentEvent = productDealerChangedEvents[i];
            tasks[i] = EnrichProductDealerChangedEvent(currentEvent, ct);
        }

        return await Task.WhenAll(tasks);
    }
}