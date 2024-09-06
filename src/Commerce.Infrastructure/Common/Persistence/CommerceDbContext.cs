using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Product = Commerce.Domain.Products.Product;

namespace Commerce.Infrastructure.Common.Persistence;

public class CommerceDbContext : DbContext
{
    public static readonly string DefaultSchema = Environment.GetEnvironmentVariable("SchemaName") ?? "commerce";

    public CommerceDbContext(
        DbContextOptions<CommerceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomerProductLink> CustomerProducts => Set<CustomerProductLink>();
    public DbSet<CustomerOrderLink> CustomerOrders => Set<CustomerOrderLink>();
    public DbSet<ProductOrderLink> ProductOrders => Set<ProductOrderLink>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CommerceDbContext).Assembly);

        builder.HasDefaultSchema(DefaultSchema);

        base.OnModelCreating(builder);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken ct = default)
    {
        var result = await SaveChangesAsync(ct);

        if (result > 0) return true;
        
        return false;
    }
}