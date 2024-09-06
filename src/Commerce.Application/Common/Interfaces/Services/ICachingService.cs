using System.Diagnostics.CodeAnalysis;

namespace Commerce.Application.Common.Interfaces.Services;

public interface ICachingService
{
    void Store<TEntity>(string id, TEntity? entity);
    bool TryGetValue<TEntity>(string id, [NotNullWhen(true)] out TEntity? entity) where TEntity : class;
    void ClearEntityCache();
    void Dispose();
}