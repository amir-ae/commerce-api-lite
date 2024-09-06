using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Infrastructure.Customers.Repositories;
using Commerce.Infrastructure.Products.Repositories;
using Marten;

namespace Commerce.Infrastructure.Common.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _session;

    public ICachingService CachingService { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IProductRepository ProductRepository { get; }

    public UnitOfWork(IDocumentSession session, ICachingService cachingService)
    {
        _session = session;
        CachingService = cachingService;
        CustomerRepository = new CustomerMartenRepository(_session, CachingService);
        ProductRepository = new ProductMartenRepository(_session, CachingService);
    }

    public void Store<TEntity>(string id, TEntity? entity)
        => CachingService.Store(id, entity);
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);

        CachingService.ClearEntityCache();
    }

    public void Dispose()
    {
        try
        {
            CachingService.Dispose();
            CustomerRepository.Dispose();
            ProductRepository.Dispose();
            _session.Dispose();
        }
        catch
        {
            //ignore
        }
    }
}