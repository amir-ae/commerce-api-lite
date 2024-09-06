using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Commerce.Domain.Products.Events;
using Commerce.Domain.Products.ValueObjects;

namespace Commerce.Application.Common.Interfaces.Persistence;

public interface IProductRepository : IDisposable
{
    Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> ByStreamIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByIdAsync(ProductId id, CancellationToken ct = default);
    Task<ProductEvents?> EventsByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByOrderIdAsync(OrderId orderId, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<List<Product>> ListAsync(CentreId? centreId = null, CancellationToken ct = default);
    Task<List<Product>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default);
    Product Create(ProductCreatedEvent productCreatedEvent);
    void Append(ProductEvent productEvent);
    Task SaveChangesAsync(CancellationToken ct = default);
}