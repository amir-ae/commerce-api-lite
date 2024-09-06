using System.Diagnostics.CodeAnalysis;
using Commerce.Application.Common.Interfaces.Services;

namespace Commerce.Infrastructure.Common.Services;

public class CachingService : ICachingService
{
    private readonly Dictionary<string, object?> _entityCache = new();

    public void Store<TEntity>(string id, TEntity? entity)
    {
        _entityCache[id] = entity;
    }

    public bool TryGetValue<TEntity>(string id, [NotNullWhen(true)] out TEntity? entity) where TEntity : class
    {
        if (_entityCache.TryGetValue(id, out var cachedEntity) && cachedEntity is TEntity typedEntity)
        {
            entity = typedEntity;
            return true;
        }
        entity = null;
        return false;
    }

    public void ClearEntityCache()
    {
        _entityCache.Clear();
    }
    
    public void Dispose()
    {
        try
        {
            _entityCache.Clear();
        }
        catch
        {
            //ignore
        }
    }
}