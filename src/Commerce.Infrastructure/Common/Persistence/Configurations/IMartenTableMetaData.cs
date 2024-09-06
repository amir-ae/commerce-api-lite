using Marten;

namespace Commerce.Infrastructure.Common.Persistence.Configurations;

public interface IMartenTableMetaData
{
    void SetTableMetaData(StoreOptions storeOptions);
}