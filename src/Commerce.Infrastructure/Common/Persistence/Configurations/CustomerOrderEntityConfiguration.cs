using Commerce.Domain.Common.ValueObjects;
using Commerce.Infrastructure.Common.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Common.Persistence.Configurations;

public class CustomerOrderEntityConfiguration : IEntityTypeConfiguration<CustomerOrderLink>
{
    public void Configure(EntityTypeBuilder<CustomerOrderLink> builder)
    {
        builder
            .ToTable("customer_order")
            .HasKey(co => new { co.CustomerId, co.OrderId });

        builder
            .Property(co => co.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(36)
            .HasConversion(new CustomerIdValueConverter());

        builder
            .Property(co => co.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(50)
            .HasConversion(new OrderIdValueConverter());

        builder
            .Property(co => co.CentreId)
            .HasColumnName("centre_id")
            .HasConversion(new CentreIdValueConverter())
            .IsRequired();
    }
}