using Commerce.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Fixtures
{
    public class TestCommerceDbContext : CommerceDbContext
    {
        public TestCommerceDbContext(DbContextOptions<CommerceDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommerceDbContext).Assembly);
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }
    }
}
