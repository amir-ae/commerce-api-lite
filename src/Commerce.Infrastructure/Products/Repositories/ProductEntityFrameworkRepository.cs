using Microsoft.EntityFrameworkCore;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Extensions;

namespace Commerce.Infrastructure.Products.Repositories;

public class ProductEntityFrameworkRepository : IProductRepository
{
    private readonly Marten.IDocumentSession _session;
    private readonly CommerceDbContext _context;

    public ProductEntityFrameworkRepository(Marten.IDocumentSession session, CommerceDbContext context)
    {
        _session = session;
        _context = context;
    }
    
    private static readonly Func<CommerceDbContext, ProductId, CancellationToken, Task<bool>> CheckByIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, ProductId id, CancellationToken ct) => context.Products.AsNoTracking()
            .Any(x => x.Id == id));

    public async Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await CheckByIdFuncAsync(_context, id, ct);
    }

    private static readonly Func<CommerceDbContext, ProductId, CancellationToken, Task<Product?>> ByIdFuncAsync =
            EF.CompileAsyncQuery((CommerceDbContext context, ProductId id, CancellationToken ct) => context.Products.AsNoTracking()
                .FirstOrDefault(x => x.Id == id));
    
    public async Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await ByIdFuncAsync(_context, id, ct);
    }
    
    public async Task<Product?> ByStreamIdAsync(ProductId id, CancellationToken ct = default)
     {
         return await _session.Events.AggregateStreamAsync<Product>(id.Value, token: ct);
     }

    private static readonly Func<CommerceDbContext, ProductId, CancellationToken, Task<Product?>> DetailByIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, ProductId id, CancellationToken ct) => context.Products.AsNoTracking()
            .Include(x => x.Customers).ThenInclude(pc => pc.Customer)
            .FirstOrDefault(x => x.Id == id));
     
    public async Task<Product?> DetailByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await DetailByIdFuncAsync(_context, id, ct);
    }
    
    private static readonly Func<CommerceDbContext, OrderId, CancellationToken, Task<Product?>> DetailByOrderIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, OrderId orderId, CancellationToken ct) => context.Products.AsNoTracking()
            .Include(x => x.Customers).ThenInclude(pc => pc.Customer)
            .FirstOrDefault(x => x.Orders.Any(po => po.OrderId == orderId)));
    
    public async Task<Product?> DetailByOrderIdAsync(OrderId orderId, CancellationToken ct = default)
    {
        return await DetailByOrderIdFuncAsync(_context, orderId, ct);
    }
    
    private static readonly Func<CommerceDbContext, CentreId, IAsyncEnumerable<Product>> DetailByCentreIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CentreId centreId) => context.Products.AsNoTracking()
            .Include(x => x.Customers).ThenInclude(pc => pc.Customer)
            .Where(x => x.IsDeleted != true && x.Orders.Any(po => po.CentreId == centreId)));
    
    public async Task<List<Product>> DetailByCentreIdAsync(CentreId centreId, CancellationToken ct = default)
    {
        var result = new List<Product>();
        await foreach (var item in DetailByCentreIdFuncAsync(_context, centreId).WithCancellation(ct)) 
        {
            result.Add(item);
        }
        return result
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.LastModifiedAt)
            .ToList();
    }
    
    public async Task<ProductEvents?> EventsByIdAsync(ProductId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.FirstOrDefault(x => x.EventType == typeof(ProductCreatedEvent))?.Data as ProductCreatedEvent;
        if (createdEvent is null) return null;
        return events.Sort(new ProductEvents(createdEvent));
    }

    public async Task<List<Product>> DetailByIdsAsync(List<ProductId> ids, CancellationToken ct = default)
    {
        return await _context.Products.AsNoTracking()
            .Include(x => x.Customers)
            .ThenInclude(pc => pc.Customer)
            .Where(x => ids.Contains(x.Id)).ToListAsync(ct);
    }
    
    private class ProductWrapper
    {
        public Product Product { get; init; } = null!;
        public int TotalCount { get; init; }
    }

    private static readonly Func<CommerceDbContext, int, int, IAsyncEnumerable<ProductWrapper>> ByPageFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize) => context.Products.AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(p => new ProductWrapper { Product = p, TotalCount = context.Products.AsNoTracking().Count() }));
    
    private static readonly Func<CommerceDbContext, int, int, CentreId, IAsyncEnumerable<ProductWrapper>> ByCentrePageFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize, CentreId centreId) => context.Products.AsNoTracking()
            .Where(x => x.IsDeleted != true && x.Orders.Any(co => co.CentreId == centreId))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(p => new ProductWrapper { Product = p, TotalCount = context.Products.AsNoTracking().Count() }));

    public async Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId,
        CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<ProductWrapper>();

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
            IQueryable<Product> query = _context.Products.AsNoTracking().Where(p => p.IsDeleted != true);
                
            if (centreId is not null)
            {
                query = query.Where(e => e.Orders.Any(po => po.CentreId == centreId));
            }
            
            if (nextPage == true)
            {
                result = await query.Where(p => p.CreatedAt < keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                            || !p.LastModifiedAt.HasValue || p.LastModifiedAt < keyRecord.LastModifiedAt))
                        .OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.LastModifiedAt).Take(pageSize)
                        .Select(p => new ProductWrapper { Product = p, TotalCount = _context.Products.AsNoTracking().Count() }).ToListAsync(ct);
            }
            else
            {
                result = await query.Where(p => p.CreatedAt > keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                            || !p.LastModifiedAt.HasValue || p.LastModifiedAt > keyRecord.LastModifiedAt))
                        .OrderBy(p => p.CreatedAt).ThenBy(p => p.LastModifiedAt).Take(pageSize)
                        .Select(p => new ProductWrapper { Product = p, TotalCount = _context.Products.AsNoTracking().Count() }).ToListAsync(ct);

                result = result.OrderByDescending(p => p.Product.CreatedAt).ThenByDescending(p => p.Product.LastModifiedAt).ToList();
            }
        }

        var products = result.Select(p => p.Product).ToList();
        var totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

        return (products, totalCount);
    }

    private static readonly Func<CommerceDbContext, int, int, IAsyncEnumerable<ProductWrapper>> ByPageDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize) => context.Products.AsNoTracking()
            .Include(x => x.Customers).ThenInclude(pc => pc.Customer)
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(p => new ProductWrapper { Product = p, TotalCount = context.Products.AsNoTracking().Count() }));
    
    private static readonly Func<CommerceDbContext, int, int, CentreId, IAsyncEnumerable<ProductWrapper>> ByCentrePageDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, int skipSize, int pageSize, CentreId centreId) => context.Products.AsNoTracking()
            .Include(x => x.Customers).ThenInclude(pc => pc.Customer)
            .Where(x => x.IsDeleted != true && x.Orders.Any(po => po.CentreId == centreId))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .Skip(skipSize)
            .Take(pageSize)
            .Select(p => new ProductWrapper { Product = p, TotalCount = context.Products.AsNoTracking().Count() }));

    public async Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId,
        CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<ProductWrapper>();

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
            IQueryable<Product> query = _context.Products.AsNoTracking().Where(p => p.IsDeleted != true);
                
            if (centreId is not null)
            {
                query = query.Where(x => x.Orders.Any(po => po.CentreId == centreId));
            }
            
            if (nextPage == true)
            {
                result = await query.Include(p => p.Customers).ThenInclude(pc => pc.Customer)
                        .Where(p => p.CreatedAt < keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                                                                           || !p.LastModifiedAt.HasValue || p.LastModifiedAt < keyRecord.LastModifiedAt))
                        .OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.LastModifiedAt).Take(pageSize)
                        .Select(p => new ProductWrapper { Product = p, TotalCount = _context.Products.AsNoTracking().Count() }).ToListAsync(ct);
            }
            else
            {
                result = await query.Include(p => p.Customers).ThenInclude(pc => pc.Product)
                        .Where(p => p.CreatedAt > keyRecord!.CreatedAt && (!keyRecord.LastModifiedAt.HasValue 
                                                                           || !p.LastModifiedAt.HasValue || p.LastModifiedAt > keyRecord.LastModifiedAt))
                        .OrderBy(p => p.CreatedAt).ThenBy(p => p.LastModifiedAt).Take(pageSize)
                        .Select(p => new ProductWrapper { Product = p, TotalCount = _context.Products.AsNoTracking().Count() }).ToListAsync(ct);

                result = result.OrderByDescending(p => p.Product.CreatedAt).ThenByDescending(p => p.Product.LastModifiedAt).ToList();
            }
        }

        var products = result.Select(p => p.Product).ToList();
        var totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

        return (products, totalCount);
    }

    private static readonly Func<CommerceDbContext, IAsyncEnumerable<Product>> ListFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context) => context.Products.AsNoTracking()
            .Where(product => product.IsDeleted != true));
    
    private static readonly Func<CommerceDbContext, CentreId, IAsyncEnumerable<Product>> ByCentreIdFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CentreId centreId) => context.Products.AsNoTracking()
            .Where(product => product.IsDeleted != true 
                               && product.Orders.Any(po => po.CentreId == centreId)));
    
    public async Task<List<Product>> ListAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<Product>();
        await foreach (var item in centreId is null
                           ? ListFuncAsync(_context).WithCancellation(ct)
                           : ByCentreIdFuncAsync(_context, centreId).WithCancellation(ct)) 
        {
            result.Add(item);
        }
        return result.ToList();
    }

    private static readonly Func<CommerceDbContext, IAsyncEnumerable<Product>> ListDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context) => context.Products.AsNoTracking()
            .Include(p => p.Customers).ThenInclude(pc => pc.Customer)
            .Where(product => product.IsDeleted != true));
    
    private static readonly Func<CommerceDbContext, CentreId, IAsyncEnumerable<Product>> ByCentreIdDetailFuncAsync =
        EF.CompileAsyncQuery((CommerceDbContext context, CentreId centreId) => context.Products.AsNoTracking()
            .Include(p => p.Customers).ThenInclude(pc => pc.Customer)
            .Where(product => product.IsDeleted != true 
                               && product.Orders.Any(po => po.CentreId == centreId)));
    
    public async Task<List<Product>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var result = new List<Product>();
        await foreach (var item in centreId is null
                           ? ListDetailFuncAsync(_context).WithCancellation(ct)
                           : ByCentreIdDetailFuncAsync(_context, centreId).WithCancellation(ct)) 
        {
            result.Add(item);
        }
        return result.ToList();
    }

    public Product Create(ProductCreatedEvent productCreatedEvent)
    {
        var productId = productCreatedEvent.ProductId;
        _session.Events.StartStream<Product>(productId.Value, productCreatedEvent);
        return Product.Create(productCreatedEvent);
    }

    public async Task<Product> CreateAsync(ProductCreatedEvent productCreatedEvent, CancellationToken ct = default)
    {
        var productId = productCreatedEvent.ProductId;
        _session.Events.StartStream<Product>(productId.Value, productCreatedEvent);
        await _session.SaveChangesAsync(ct);
        return (await ByStreamIdAsync(productId, ct))!;
    }

    public void Append(ProductEvent productEvent)
    {
        var productId = productEvent.ProductId.Value;
        _session.Events.Append(productId, productEvent);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _session.Dispose();
        _context.Dispose();
    }
}
