using System.Reflection;
using System.Text.Json.Serialization;
using Commerce.Domain.Common.JsonConverters;
using Commerce.Domain.Customers;
using Commerce.Domain.Products;
using Commerce.Fixtures.Data;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Persistence.Configurations;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;
using CustomerRole = Commerce.Domain.Customers.ValueObjects.CustomerRole;

namespace Commerce.Fixtures;

public class CommerceApplicationFactory<Program> : WebApplicationFactory<Program> where Program : class
{
    private readonly string _connectionString =
        "server=localhost; port=5432; timeout=15; pooling=True; minpoolsize=1; maxpoolsize=100; commandtimeout= 20; database=CommerceTests; user id=postgres; password=T1VWLjZIofw60dVeYI2s";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .ConfigureTestServices(services =>
            {
                var options = new DbContextOptionsBuilder<CommerceDbContext>()
                    .UseNpgsql(_connectionString, serverOptions =>
                    {
                        serverOptions.MigrationsAssembly(typeof(CommerceDbContext).Assembly.FullName);
                        serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    })
                    .Options;

                services.AddScoped<CommerceDbContext>(
                    _ => new TestCommerceDbContext(options));

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<CommerceDbContext>();
                db.Database.EnsureCreated();
                
                var serializer = new Marten.Services.JsonNetSerializer();
                serializer.Configure(c =>
                {
                    c.Converters.Add(new SmartEnumJsonConverter<CustomerRole>());
                    c.Converters.Add(new StronglyTypedIdJsonConverter());
                    c.ContractResolver = new ResolvePrivateSetters();
                });
                services.AddDbContextPool<CommerceDbContext>((serviceProvider, contextOptions) =>
                {
                    contextOptions.UseNpgsql(_connectionString, serverOptions =>
                    {
                        serverOptions.MigrationsAssembly(typeof(CommerceDbContext).Assembly.FullName);
                        serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                });
                services.AddMarten(storeOptions =>
                    {
                        storeOptions.DatabaseSchemaName = "commerce";
                        storeOptions.Connection(_connectionString);
                        storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                        storeOptions.Serializer(serializer);
                        storeOptions.CreateDatabasesForTenants(c =>
                        {
                            c.MaintenanceDatabase("host=localhost; user id=postgres; password=T1VWLjZIofw60dVeYI2s");
                            c.ForTenant()
                                .CheckAgainstPgDatabase()
                                .WithOwner("postgres")
                                .WithEncoding("UTF-8")
                                .ConnectionLimit(-1);
                        });

                        var stuff = Assembly.GetExecutingAssembly()
                            .DefinedTypes.Where(type => type.IsSubclassOf(typeof(MartenTableMetaDataBase))).ToList();

                        foreach (Type type in stuff)
                        {
                            IMartenTableMetaData temp = (IMartenTableMetaData)Activator.CreateInstance(type)!;
                            temp.SetTableMetaData(storeOptions);
                        }
                        
                        storeOptions.Events.StreamIdentity = StreamIdentity.AsString;
                        storeOptions.Schema.For<Customer>().Identity(x => x.AggregateId);
                        storeOptions.Schema.For<Customer>().UseNumericRevisions(true);
                        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
                        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
                        storeOptions.Projections.Snapshot<Customer>(SnapshotLifecycle.Inline);
                        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
                    })
                    .ApplyAllDatabaseChangesOnStartup()
                    .InitializeWith(new InitialData(InitialDatasets.InitialData))
                    .UseLightweightSessions()
                    .AddAsyncDaemon(DaemonMode.Solo);
                
                services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
                services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            });
    }
}