using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Commerce.Fixtures;

[TestCaseOrderer("Commerce.Fixtures.PriorityOrderer", "Commerce.Fixtures")]
public class CommerceTests : IClassFixture<CommerceDbContextFactory>, IClassFixture<CommerceApplicationFactory<Program>>
{
    protected readonly CommerceApplicationFactory<Program> Factory;
    protected IDocumentStore DocumentStore => Factory.Services.GetRequiredService<IDocumentStore>();

    protected CommerceTests(CommerceApplicationFactory<Program> factory)
    {
        Factory = factory;
    }

    /// <summary>
    /// 1. Start generation of projections
    /// 2. Wait for projections to be projected
    /// </summary>
    protected async Task GenerateProjectionsAsync()
    {
        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        await daemon.WaitForNonStaleData(5.Seconds());
    }
        
    protected async Task ResetAllDataAsync()
    {
        await DocumentStore.Advanced.ResetAllData();
    }

    protected IDocumentSession OpenSession() => DocumentStore.LightweightSession();
}