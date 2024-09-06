using Commerce.Domain.Products;
using Commerce.Infrastructure.Common.Persistence.Configurations;
using Marten.Events.Projections;
using Marten;

namespace Commerce.Infrastructure.Products.Configurations;

public class MartenTableMetaDataProduct : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
    }
}