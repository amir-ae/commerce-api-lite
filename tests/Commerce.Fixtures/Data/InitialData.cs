using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Events;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Marten;
using Marten.Schema;

namespace Commerce.Fixtures.Data;

public class InitialData: IInitialData
{
    private readonly object[] _initialData;

    public InitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken ct)
    {
        await using var session = store.LightweightSession();
        
        if (session.Query<Customer>().Any() || session.Query<Product>().Any())
        {
            await store.Advanced.Clean.CompletelyRemoveAllAsync(ct);
        }

        foreach (var @event in _initialData)
        {
            switch (@event)
            {
                case ProductCreatedEvent created:
                    session.Events.StartStream<Product>(created.ProductId.Value, created);
                    break;
                case CustomerCreatedEvent created:
                    session.Events.StartStream<Customer>(created.CustomerId.Value, created);
                    break;
            }
        }
        await session.SaveChangesAsync(ct);
        
        foreach (var @event in _initialData)
        {
            switch (@event)
            {
                case ProductDeletedEvent deleted:
                    session.Events.Append(deleted.ProductId.Value, deleted);
                    break;
                case CustomerDeletedEvent deleted:
                    session.Events.Append(deleted.CustomerId.Value, deleted);
                    break;
            }
        }
        await session.SaveChangesAsync(ct);
    }
}

public static class InitialDatasets
{
    public static readonly object[] InitialData =
    {
        new CustomerCreatedEvent(
            new CustomerId("1"),
            "Rae",
            null,
            "Mccall",
            null,
            new PhoneNumber("(321) 682-8918"),
            new CityId(1),
            "San Gregorio",
            null,
            new HashSet<ProductId> {new("PA")},
            new HashSet<OrderId> { new("A", "14424963-25e4-4731-8501-461750d27037") },
            new AppUserId(Guid.NewGuid())),
        new CustomerCreatedEvent(
            new CustomerId("2"),
            "Aspen",
            null,
            "Coffey",
            null,
            new PhoneNumber("514-374-2937"),
            new CityId(1),
            "Klerksdorp",
            null,
            new HashSet<ProductId> {new("PB"), new("PC")},
            null,
            new AppUserId(Guid.NewGuid())),
        new CustomerCreatedEvent(
            new CustomerId("3"),
            "Leonard",
            null,
            "Sandoval",
            null,
            new PhoneNumber("(576) 452-6868"),
            new CityId(2),
            "Inírida",
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new CustomerDeletedEvent(
            new CustomerId("3"),
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("PA"),
            "TCL",
            "L40S60A",
            null,
            new CustomerId("1"),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new HashSet<OrderId> { new("A", "14424963-25e4-4731-8501-461750d27037") },
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("PB"),
            "POLARLINE",
            "32PL13TC-SM",
            null,
            new CustomerId("2"),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("PC"),
            "STARWIND",
            "SW-LED40BA201",
            null,
            new CustomerId("2"),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductDeletedEvent(
            new ProductId("PC"),
            new AppUserId(Guid.NewGuid()))
    };
}