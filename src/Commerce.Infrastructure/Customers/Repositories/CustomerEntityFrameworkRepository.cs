using Microsoft.EntityFrameworkCore;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Customers.Events;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Extensions;

namespace Commerce.Infrastructure.Customers.Repositories;

public class CustomerEntityFrameworkRepository : ICustomerRepository
{
    private readonly Marten.IDocumentSession _session;
    private readonly CommerceDbContext _context;

    public CustomerEntityFrameworkRepository(Marten.IDocumentSession session, CommerceDbContext context)
    {
        _session = session;
        _context = context;
    }
    
    private static readonly Func<CommerceDbContext, CustomerId, CancellationToken, Task<bool>> CheckByIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CustomerId id, CancellationToken ct) => context.Customers.AsNoTracking()
            .Any(x => x.Id == id));

    public async Task<bool> CheckByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await CheckByIdFuncAsync(_context, id, ct);
    }

    private static readonly Func<CommerceDbContext, CustomerId, CancellationToken, Task<Customer?>> ByIdFuncAsync =
            EF.CompileAsyncQuery((CommerceDbContext context, CustomerId id, CancellationToken ct) => context.Customers.AsNoTracking()
                .FirstOrDefault(x => x.Id == id));
    
    public async Task<Customer?> ByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await ByIdFuncAsync(_context, id, ct);
    }
    
    public async Task<Customer?> ByStreamIdAsync(CustomerId id, CancellationToken ct = default)
     {
         return await _session.Events.AggregateStreamAsync<Customer>(id.Value, token: ct);
     }
     
    private static readonly Func<CommerceDbContext, string, string, CancellationToken, Task<Customer?>> ByDataFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, string fullName, string phoneNumber, CancellationToken ct) => context.Customers.AsNoTracking()
            .FirstOrDefault(x => x.FullName == fullName && x.PhoneNumber.Value == phoneNumber));
    
    public async Task<Customer?> ByDataAsync(string fullName, string phoneNumber, CancellationToken ct = default)
    {
        return await ByDataFuncAsync(_context, fullName, phoneNumber, ct);
    }
    
    private static readonly Func<CommerceDbContext, CustomerId, CancellationToken, Task<Customer?>> DetailByIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CustomerId id, CancellationToken ct) => context.Customers.AsNoTracking()
            .Include(x => x.Products).ThenInclude(cp => cp.Product)
            .FirstOrDefault(x => x.Id == id));
     
    public async Task<Customer?> DetailByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await DetailByIdFuncAsync(_context, id, ct);
    }
    
    public async Task<CustomerEvents?> EventsByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.FirstOrDefault(x => x.EventType == typeof(CustomerCreatedEvent))?.Data as CustomerCreatedEvent;
        if (createdEvent is null) return null;
        return events.Sort(new CustomerEvents(createdEvent));
    }

    public async Task<List<Customer>> DetailByIdsAsync(List<CustomerId> ids, CancellationToken ct = default)
    {
        return await _context.Customers.AsNoTracking()
            .Include(x => x.Products)
            .ThenInclude(cp => cp.Product)
            .Where(x => ids.Contains(x.Id)).ToListAsync(ct);
    }
    
    private class CustomerWrapper
    {
        public Customer Customer { get; init; } = null!;
        public int TotalCount { get; init; }
    }

    private static readonly Func<CommerceDbContext, int, int, IAsyncEnumerable<CustomerWrapper>> ByPageFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize) => context.Customers.AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(c => new CustomerWrapper { Customer = c, TotalCount = context.Customers.AsNoTracking().Count() }));
    
    private static readonly Func<CommerceDbContext, int, int, CentreId, IAsyncEnumerable<CustomerWrapper>> ByCentrePageFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize, CentreId centreId) => context.Customers.AsNoTracking().AsNoTracking()
            .Where(x => x.IsDeleted != true && x.Orders.Any(co => co.CentreId == centreId))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(c => new CustomerWrapper { Customer = c, TotalCount = context.Customers.AsNoTracking().Count() }));

    public async Task<(List<Customer>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, CustomerId? keyId,
        CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<CustomerWrapper>();

        if (!nextPage.HasValue || keyId is null)
        {
            var skipSize = pageSize * (pageNumber - 1);
            await foreach (var item in centreId is null
                               ? ByPageFuncAsync(_context, skipSize, pageSize).WithCancellation(ct)
                               : ByCentrePageFuncAsync(_context, skipSize, pageSize, centreId).WithCancellation(ct)) 
            {
                result.Add(item);
            }
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            IQueryable<Customer> query = _context.Customers.AsNoTracking().Where(c => c.IsDeleted != true);
                
            if (centreId is not null)
            {
                query = query.Where(e => e.Orders.Any(c => c.CentreId == centreId));
            }
            
            if (nextPage == true)
            {
                result = await query.Where(c => c.CreatedAt < keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                            || !c.LastModifiedAt.HasValue || c.LastModifiedAt < keyRecord.LastModifiedAt))
                        .OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.LastModifiedAt).Take(pageSize)
                        .Select(c => new CustomerWrapper { Customer = c, TotalCount = _context.Customers.AsNoTracking().Count() }).ToListAsync(ct);
            }
            else
            {
                result = await query.Where(c => c.CreatedAt > keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                            || !c.LastModifiedAt.HasValue || c.LastModifiedAt > keyRecord.LastModifiedAt))
                        .OrderBy(c => c.CreatedAt).ThenBy(c => c.LastModifiedAt).Take(pageSize)
                        .Select(c => new CustomerWrapper { Customer = c, TotalCount = _context.Customers.AsNoTracking().Count() }).ToListAsync(ct);

                result = result.OrderByDescending(c => c.Customer.CreatedAt).ThenByDescending(c => c.Customer.LastModifiedAt).ToList();
            }
        }

        var customers = result.Select(c => c.Customer).ToList();
        var totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

        return (customers, totalCount);
    }

    private static readonly Func<CommerceDbContext, int, int, IAsyncEnumerable<CustomerWrapper>> ByPageDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize) => context.Customers.AsNoTracking()
            .Include(x => x.Products).ThenInclude(cp => cp.Product)
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(c => new CustomerWrapper { Customer = c, TotalCount = context.Customers.AsNoTracking().Count() }));
    
    private static readonly Func<CommerceDbContext, int, int, CentreId, IAsyncEnumerable<CustomerWrapper>> ByCentrePageDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize, CentreId centreId) => context.Customers.AsNoTracking()
            .Include(x => x.Products).ThenInclude(cp => cp.Product)
            .Where(x => x.IsDeleted != true && x.Orders.Any(co => co.CentreId == centreId))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(c => new CustomerWrapper { Customer = c, TotalCount = context.Customers.AsNoTracking().Count() }));

    public async Task<(List<Customer>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, CustomerId? keyId,
        CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<CustomerWrapper>();

        if (!nextPage.HasValue || keyId is null)
        {
            var skipSize = pageSize * (pageNumber - 1);
            await foreach (var item in centreId is null
                               ? ByPageDetailFuncAsync(_context, skipSize, pageSize).WithCancellation(ct)
                               : ByCentrePageDetailFuncAsync(_context, skipSize, pageSize, centreId).WithCancellation(ct)) 
            {
                result.Add(item);
            }
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            IQueryable<Customer> query = _context.Customers.AsNoTracking().Where(c => c.IsDeleted != true);
                
            if (centreId is not null)
            {
                query = query.Where(x => x.Orders.Any(co => co.CentreId == centreId));
            }
            
            if (nextPage == true)
            {
                result = await query.Include(c => c.Products).ThenInclude(cp => cp.Product)
                        .Where(c => c.CreatedAt < keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                                                                           || !c.LastModifiedAt.HasValue || c.LastModifiedAt < keyRecord.LastModifiedAt))
                        .OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.LastModifiedAt).Take(pageSize)
                        .Select(c => new CustomerWrapper { Customer = c, TotalCount = _context.Customers.AsNoTracking().Count() }).ToListAsync(ct);
            }
            else
            {
                result = await query.Include(c => c.Products).ThenInclude(cp => cp.Product)
                        .Where(c => c.CreatedAt > keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                                                                           || !c.LastModifiedAt.HasValue || c.LastModifiedAt > keyRecord.LastModifiedAt))
                        .OrderBy(c => c.CreatedAt).ThenBy(c => c.LastModifiedAt).Take(pageSize)
                        .Select(c => new CustomerWrapper { Customer = c, TotalCount = _context.Customers.AsNoTracking().Count() }).ToListAsync(ct);

                result = result.OrderByDescending(c => c.Customer.CreatedAt).ThenByDescending(c => c.Customer.LastModifiedAt).ToList();
            }
        }

        var customers = result.Select(c => c.Customer).ToList();
        var totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

        return (customers, totalCount);
    }

    private static readonly Func<CommerceDbContext, IAsyncEnumerable<Customer>> ListFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context) => context.Customers.AsNoTracking()
            .Where(customer => customer.IsDeleted != true));
    
    private static readonly Func<CommerceDbContext, CentreId, IAsyncEnumerable<Customer>> ByCentreIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CentreId centreId) => context.Customers.AsNoTracking()
            .Where(customer => customer.IsDeleted != true 
                               && customer.Orders.Any(co => co.CentreId == centreId)));
    
    public async Task<List<Customer>> ListAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<Customer>();
        await foreach (var item in centreId is null
                           ? ListFuncAsync(_context).WithCancellation(ct)
                           : ByCentreIdFuncAsync(_context, centreId).WithCancellation(ct)) 
        {
            result.Add(item);
        }
        return result.ToList();
    }

    private static readonly Func<CommerceDbContext, IAsyncEnumerable<Customer>> ListDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context) => context.Customers.AsNoTracking()
            .Include(c => c.Products).ThenInclude(cp => cp.Product)
            .Where(customer => customer.IsDeleted != true));
    
    private static readonly Func<CommerceDbContext, CentreId, IAsyncEnumerable<Customer>> ByCentreIdDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CentreId centreId) => context.Customers.AsNoTracking()
            .Include(c => c.Products).ThenInclude(cp => cp.Product)
            .Where(customer => customer.IsDeleted != true 
                               && customer.Orders.Any(co => co.CentreId == centreId)));
    
    public async Task<List<Customer>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<Customer>();
        await foreach (var item in centreId is null
                           ? ListDetailFuncAsync(_context).WithCancellation(ct)
                           : ByCentreIdDetailFuncAsync(_context, centreId).WithCancellation(ct)) 
        {
            result.Add(item);
        }
        return result.ToList();
    }

    public Customer Create(CustomerCreatedEvent customerCreatedEvent)
    {
        var customerId = customerCreatedEvent.CustomerId;
        _session.Events.StartStream<Customer>(customerId.Value, customerCreatedEvent);
        return Customer.Create(customerCreatedEvent);
    }

    public async Task<Customer> CreateAsync(CustomerCreatedEvent customerCreatedEvent, CancellationToken ct = default)
    {
        var customerId = customerCreatedEvent.CustomerId;
        _session.Events.StartStream<Customer>(customerId.Value, customerCreatedEvent);
        await _session.SaveChangesAsync(ct);
        return (await ByStreamIdAsync(customerId, ct))!;
    }

    public void Append(CustomerEvent customerEvent)
    {
        var customerId = customerEvent.CustomerId.Value;
        _session.Events.Append(customerId, customerEvent);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }

    public async Task<bool> Seed(IEnumerable<CustomerCreatedEvent> initialData, CancellationToken ct = default)
    {
        if (!(await ListAsync(null, ct)).Any())
        {
            foreach (var createdEvent in initialData)
            {
                _session.Events.StartStream<Customer>(createdEvent.CustomerId.Value, createdEvent);
            }
            await _session.SaveChangesAsync(ct);
            
            Console.WriteLine("---> Database Customers seeded.");
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        _session.Dispose();
        _context.Dispose();
    }
}
