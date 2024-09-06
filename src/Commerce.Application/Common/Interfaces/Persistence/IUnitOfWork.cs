using Commerce.Application.Common.Interfaces.Services;

namespace Commerce.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICachingService CachingService { get; }
    ICustomerRepository CustomerRepository { get; }
    IProductRepository ProductRepository { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
}