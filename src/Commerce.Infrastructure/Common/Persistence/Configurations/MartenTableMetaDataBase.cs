using Marten;
using Marten.Events;

namespace Commerce.Infrastructure.Common.Persistence.Configurations;

public abstract class MartenTableMetaDataBase : IMartenTableMetaData
{
    public void SetTableMetaData(StoreOptions storeOptions)
    {
        SetSpecificTableMetaData(storeOptions);
        storeOptions.Events.StreamIdentity = StreamIdentity.AsString;
    }

    protected abstract void SetSpecificTableMetaData(StoreOptions storeOptions);
}