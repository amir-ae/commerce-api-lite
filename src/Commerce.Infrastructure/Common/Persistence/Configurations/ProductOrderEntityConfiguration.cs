using Commerce.Domain.Common.ValueObjects;
using Commerce.Infrastructure.Common.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Common.Persistence.Configurations;

public class ProductOrderEntityConfiguration : IEntityTypeConfiguration<ProductOrderLink>
{
    public void Configure(EntityTypeBuilder<ProductOrderLink> builder)
    {
        builder
            .ToTable("product_order")
            .HasKey(po => new { po.ProductId, po.OrderId });

        builder
            .Property(po => po.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(50)
            .HasConversion(new ProductIdValueConverter());

        builder
            .Property(po => po.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(50)
            .HasConversion(new OrderIdValueConverter());
        
        builder
            .Property(po => po.CentreId)
            .HasColumnName("centre_id")
            .HasConversion(new CentreIdValueConverter())
            .IsRequired();
    }
}