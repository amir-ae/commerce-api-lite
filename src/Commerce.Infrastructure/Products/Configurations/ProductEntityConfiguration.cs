using Commerce.Domain.Products;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Persistence.ValueConverters;
using Commerce.Infrastructure.Products.ValueConverters;
using Commerce.Infrastructure.Common.Extensions;
using Commerce.Infrastructure.Customers.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Products.Configurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", CommerceDbContext.DefaultSchema);
        
        builder.Ignore(p => p.OrderIds);
        
        builder.HasKey(p => p.Id);
        
        builder
            .Property(p => p.Id)
            .HasColumnName(nameof(Product.Id).ToSnakeCase())
            .ValueGeneratedOnAdd()
            .HasMaxLength(50)
            .HasConversion(new ProductIdValueConverter());

        builder.Property(p => p.Brand)
            .HasColumnName(nameof(Product.Brand).ToSnakeCase())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Model)
            .HasColumnName(nameof(Product.Model).ToSnakeCase())
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.SerialId)
            .HasColumnName(nameof(Product.SerialId).ToSnakeCase())
            .HasConversion(new NullableSerialIdValueConverter());
        
        builder.Property(p => p.OwnerId)
            .HasColumnName(nameof(Product.OwnerId).ToSnakeCase())
            .HasMaxLength(36)
            .HasConversion(new NullableCustomerIdValueConverter());
        
        builder.Property(p => p.DealerId)
            .HasColumnName(nameof(Product.DealerId).ToSnakeCase())
            .HasMaxLength(36)
            .HasConversion(new NullableCustomerIdValueConverter());
        
        builder.Property(p => p.DeviceType)
            .HasColumnName(nameof(Product.DeviceType).ToSnakeCase())
            .HasMaxLength(30);
        
        builder.Property(p => p.PanelModel)
            .HasColumnName(nameof(Product.PanelModel).ToSnakeCase())
            .HasMaxLength(50);
        
        builder.Property(p => p.PanelSerialNumber)
            .HasColumnName(nameof(Product.PanelSerialNumber).ToSnakeCase())
            .HasMaxLength(100);
        
        builder.Property(p => p.WarrantyCardNumber)
            .HasColumnName(nameof(Product.WarrantyCardNumber).ToSnakeCase())
            .HasMaxLength(50);

        builder.Property(p => p.DateOfPurchase)
            .HasColumnName(nameof(Product.DateOfPurchase).ToSnakeCase())
            .HasConversion(new NullableUtcDateTimeOffsetValueConverter());
        
        builder.Property(p => p.InvoiceNumber)
            .HasColumnName(nameof(Product.InvoiceNumber).ToSnakeCase())
            .HasMaxLength(50);
        
        builder.Property(p => p.PurchasePrice)
            .HasColumnName(nameof(Product.PurchasePrice).ToSnakeCase());
        
        builder.Property(b => b.IsUnrepairable)
            .HasColumnName(nameof(Product.IsUnrepairable).ToSnakeCase())
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(p => p.DateOfDemandForCompensation)
            .HasColumnName(nameof(Product.DateOfDemandForCompensation).ToSnakeCase())
            .HasConversion(new NullableUtcDateTimeOffsetValueConverter());
        
        builder.Property(p => p.DemanderFullName)
            .HasColumnName(nameof(Product.DemanderFullName).ToSnakeCase())
            .HasMaxLength(150);

        builder.Property(b => b.CreatedAt)
            .HasColumnName(nameof(Product.CreatedAt).ToSnakeCase())
            .HasConversion(new UtcDateTimeOffsetValueConverter())
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasColumnName(nameof(Product.CreatedBy).ToSnakeCase())
            .HasConversion(new AppUserIdValueConverter())
            .IsRequired();

        builder.Property(b => b.LastModifiedAt)
            .HasColumnName(nameof(Product.LastModifiedAt).ToSnakeCase())
            .HasConversion(new NullableUtcDateTimeOffsetValueConverter());

        builder.Property(b => b.LastModifiedBy)
            .HasColumnName(nameof(Product.LastModifiedBy).ToSnakeCase())
            .HasConversion(new NullableAppUserIdValueConverter());

        builder
            .Property(m => m.AggregateId)
            .HasColumnName(nameof(Product.AggregateId).ToSnakeCase())
            .HasMaxLength(50)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.Version)
            .HasColumnName(nameof(Product.Version).ToSnakeCase())
            .IsRequired();
        
        builder.Property(b => b.IsActive)
            .HasColumnName(nameof(Product.IsActive).ToSnakeCase())
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(b => b.IsDeleted)
            .HasColumnName(nameof(Product.IsDeleted).ToSnakeCase())
            .HasDefaultValue(false)
            .IsRequired();
    }
}