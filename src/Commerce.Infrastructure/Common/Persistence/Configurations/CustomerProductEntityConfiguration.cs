using Commerce.Domain.Common.ValueObjects;
using Commerce.Infrastructure.Common.Persistence.ValueConverters;
using Commerce.Infrastructure.Customers.ValueConverters;
using Commerce.Infrastructure.Products.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Common.Persistence.Configurations;

public class CustomerProductEntityConfiguration : IEntityTypeConfiguration<CustomerProductLink>
{
    public void Configure(EntityTypeBuilder<CustomerProductLink> builder)
    {
        builder
            .ToTable("customer_product")
            .HasKey(cp => new { cp.CustomerId, cp.ProductId });

        builder
            .Property(cp => cp.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(36)
            .HasConversion(new CustomerIdValueConverter());

        builder
            .Property(cp => cp.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(50)
            .HasConversion(new ProductIdValueConverter());

        builder
            .HasOne(cp => cp.Customer)
            .WithMany(c => c.Products)
            .HasForeignKey(cp => cp.CustomerId);

        builder
            .HasOne(cp => cp.Product)
            .WithMany(p => p.Customers)
            .HasForeignKey(cp => cp.ProductId);
    }
}