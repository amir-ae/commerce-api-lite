using Commerce.Domain.Customers;
using Commerce.Infrastructure.Common.Persistence.Configurations;
using Marten.Events.Projections;
using Marten;

namespace Commerce.Infrastructure.Customers.Configurations;

public class MartenTableMetaDataCustomer : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Customer>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Customer>().UseNumericRevisions(true);
        storeOptions.Projections.Snapshot<Customer>(SnapshotLifecycle.Inline);
    }
}