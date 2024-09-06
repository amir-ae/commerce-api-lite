using System.Linq.Expressions;
using Marten;
using Marten.Linq;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Commerce.Domain.Products.ValueObjects;
using Commerce.Infrastructure.Common.Extensions;
using LinqKit;
using Marten.Events.CodeGeneration;
using Marten.Pagination;

namespace Commerce.Infrastructure.Products.Repositories;

public class ProductMartenRepository : IProductRepository
{
    private readonly IDocumentSession _session;
    private readonly ICachingService _cachingService;

    public ProductMartenRepository(IDocumentSession session, ICachingService cachingService)
    {
        _session = session;
        _cachingService = cachingService;
    }

    public class CheckByIdQuery : ICompiledQuery<Product, bool>
    {
        public string Id { get; init; } = string.Empty;
        
        public Expression<Func<IMartenQueryable<Product>, bool>> QueryIs() => query 
            => query.Any(p => p.Id.Value == Id);
    }
    
    public async Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new CheckByIdQuery { Id = id.Value }, ct);
    }
    
    public class ByIdQuery : ICompiledQuery<Product, Product?>
    {
        public string Id { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Product>, Product?>> QueryIs() => query 
            => query.FirstOrDefault(c => c.Id.Value == Id);
    }
    
    public async Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByIdQuery { Id = id.Value }, ct);
    }
    
    public async Task<Product?> ByStreamIdAsync(ProductId id, CancellationToken ct = default)
    {
        var product = await _session.Events.AggregateStreamAsync<Product>(id.Value, token: ct);
        return product?.Id is null ? null : product;
    }

    public async Task<Product?> DetailByIdAsync(ProductId productId, CancellationToken ct = default)
    {
        Customer? owner = null;
        Customer? dealer = null;
        var product = await _session.Query<Product>()
            .Include<Customer>(x => owner = x).On(p => p.OwnerId!.Value)
            .Include<Customer>(x => dealer = x).On(p => p.DealerId!.Value)
            .FirstOrDefaultAsync(p => p.Id.Value == productId.Value, token: ct);

        if (product is null) return null;
        product.AddOwner(owner);
        product.AddDealer(dealer);
        
        return product;
    }

    public async Task<ProductEvents?> EventsByIdAsync(ProductId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.FirstOrDefault(x => x.EventType == typeof(ProductCreatedEvent))?.Data as ProductCreatedEvent;
        if (createdEvent is null) return null;
        return events.Sort(new ProductEvents(createdEvent));
    }

    public async Task<Product?> DetailByOrderIdAsync(OrderId orderId, CancellationToken ct = default)
    {
        Customer? owner = null;
        Customer? dealer = null;
        var product = await _session.Query<Product>()
            .Include<Customer>(x => owner = x).On(p => p.OwnerId!.Value)
            .Include<Customer>(x => dealer = x).On(p => p.DealerId!.Value)
            .FirstOrDefaultAsync(p => p.OrderIds.Any(o => o.Value == orderId.Value), token: ct);
        
        if (product is null) return null;
        product.AddOwner(owner);
        product.AddDealer(dealer);
        
        return product;
    }
    
    public async Task<List<Product>> DetailByCentreIdAsync(CentreId centreId, CancellationToken ct = default)
    {
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include(owners).On(p => p.OwnerId!.Value)
            .Include(dealers).On(p => p.DealerId!.Value)
            .Where(p => p.OrderIds.Any(o => o.CentreId == centreId.Value))
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.LastModifiedAt)
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }
    
    public class ByPageQuery : ICompiledListQuery<Product>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }
    
    public class ByCentrePageQuery : ICompiledListQuery<Product>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public string CentreId { get; init; } = string.Empty;
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.OrderIds.Any(o => o.CentreId != null! && o.CentreId == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }

    public class NextPageQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class NextCentrePageQuery : ICompiledListQuery<Product>
    {
        public string CentreId { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.OrderIds.Any(o => o.CentreId != null! && o.CentreId == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public async Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default)
    {
        List<Product> products;
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            if (centreId is null)
            {
                var query = new ByPageQuery { PageNumber = pageNumber, PageSize = pageSize };
                products = (await _session.QueryAsync(query, ct)).ToList();
                totalCount = query.Statistics.TotalResults;
            }
            else
            {
                var query = new ByCentrePageQuery { PageNumber = pageNumber, PageSize = pageSize, CentreId = centreId.Value };
                products = (await _session.QueryAsync(query, ct)).ToList();
                totalCount = query.Statistics.TotalResults;
            }
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            var result = centreId is null
                ? (await _session.QueryAsync(new NextPageQuery(), ct)).ToList()
                : (await _session.QueryAsync(new NextCentrePageQuery { CentreId = centreId.Value }, ct)).ToList();
            
            if (nextPage == true)
            {
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                result.Reverse();
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
            
            totalCount = result.Count;
        }
        
        return (products, totalCount);
    }

    public async Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, 
        ProductId? keyId, CentreId? centreId, CancellationToken ct = default)
    {
        List<Product> products;
        List<Customer> customers = new();
        long totalCount;

        var predicate = PredicateBuilder.New<Product>(x => !x.IsDeleted);
        if (centreId is not null)
        {
            predicate = predicate.And(x => x.OrderIds.Any(o => o.CentreId != null! && o.CentreId == centreId.Value));
        }
        
        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            products = (await _session.Query<Product>()
                .Include(customers).On(x => x.OwnerId!.Value)
                .Include(customers).On(x => x.DealerId!.Value)
                .Where(predicate)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Stats(out var stats)
                .ToPagedListAsync(pageNumber, pageSize, ct)).ToList();
                
            totalCount = stats.TotalResults;
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);

            var result = (await _session.Query<Product>()
                .Where(predicate)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .ToListAsync(ct)).ToList();
            
            if (nextPage == true)
            {
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                result.Reverse();
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
            
            totalCount = result.Count;
            
            var customerIds = new List<string>();
            customerIds.AddRange(products.Select(p => p.OwnerId?.Value).OfType<string>());
            customerIds.AddRange(products.Select(p => p.DealerId?.Value).OfType<string>());
        
            customers = (await _session.LoadManyAsync<Customer>(ct, customerIds.Distinct())).ToList();
        }
        
        return (AddCustomerDetailToProducts(products, customers), totalCount);
    }

    public class ListQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted);
    }
    
    public class ByCentreIdQuery : ICompiledListQuery<Product>
    {
        public string CentreId { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.OrderIds.Any(o => o.CentreId != null! && o.CentreId == CentreId));
    }
    
    public async Task<List<Product>> ListAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var products = centreId is null
            ? await _session.QueryAsync(new ListQuery(), ct)
            : await _session.QueryAsync(new ByCentreIdQuery { CentreId = centreId.Value }, ct);
        
        return products.ToList();
    }

    public async Task<List<Product>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include(owners).On(x => x.OwnerId!.Value)
            .Include(dealers).On(x => x.DealerId!.Value)
            .Where(centreId is null
                ? p => !p.IsDeleted
                : p => !p.IsDeleted && p.OrderIds.Any(o => o.CentreId == centreId.Value))
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }

    public Product Create(ProductCreatedEvent productCreatedEvent)
    {
        var productId = productCreatedEvent.ProductId.Value;
        _session.Events.StartStream<Product>(productId, productCreatedEvent);
        var product = Product.Create(productCreatedEvent);
        _cachingService.Store(productId, product);
        return product;
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

    private List<Product> AddCustomerDetailToProducts(
        IEnumerable<Product> productsEnumerable, 
        IEnumerable<Customer> customersEnumerable, 
        IEnumerable<Customer>? dealersEnumerable = null)
    {
        var products = productsEnumerable as Product[] ?? productsEnumerable.ToArray();
        var customers = customersEnumerable as Customer[] ?? customersEnumerable.ToArray();
        var dealers = dealersEnumerable as Customer[] ?? dealersEnumerable?.ToArray();
        
        var customerDictionary = customers.ToDictionary(c => c.Id);
        var dealerDictionary = (dealers ?? customers).ToDictionary(c => c.Id);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        
        Parallel.ForEach(products, parallelOptions, product =>
        {
            if (product.OwnerId is not null && customerDictionary.TryGetValue(product.OwnerId, out var owner))
            {
                product.AddOwner(owner);
            }
            if (product.DealerId is not null && dealerDictionary.TryGetValue(product.DealerId, out var dealer))
            {
                product.AddDealer(dealer);
            }
        });

        return products.ToList();
    }
    
    public void Dispose()
    {
        _session.Dispose();
    }
}
