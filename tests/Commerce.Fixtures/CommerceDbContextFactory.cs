using Commerce.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Fixtures
{
    public class CommerceDbContextFactory
    {
        public readonly TestCommerceDbContext ContextInstance;
        private readonly string _connectionString =
            "server=localhost; port=5432; timeout=15; pooling=True; minpoolsize=1; maxpoolsize=100; commandtimeout= 20; database=CommerceTests; user id=postgres; password=T1VWLjZIofw60dVeYI2s";

        public CommerceDbContextFactory()
        {
            var contextOptions = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseNpgsql(_connectionString, serverOptions =>
                {
                    serverOptions.MigrationsAssembly(typeof(CommerceDbContext).Assembly.FullName);
                    serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
                .EnableSensitiveDataLogging()
                .Options;

            EnsureCreation(contextOptions);

            ContextInstance = new TestCommerceDbContext(contextOptions);
        }

        private void EnsureCreation(DbContextOptions<CommerceDbContext> contextOptions)
        {
            using var context = new TestCommerceDbContext(contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
