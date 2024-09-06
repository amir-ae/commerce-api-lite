using Commerce.Domain.Customers.ValueObjects;
using Commerce.Infrastructure.Customers.ValueConverters;
using Commerce.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Customers.Configurations;

public static class PhoneNumberConfiguration
{
    public static void ConfigurePhoneNumber<T>(OwnedNavigationBuilder<T, PhoneNumber> builder, string propertyName) where T : class
    {
        var prefix = "";
        if (!propertyName.StartsWith(nameof(PhoneNumber)))
        {
            prefix = (propertyName.Replace(nameof(PhoneNumber), "") + "_").ToSnakeCase();
        }
        builder.Property(p => p.Value)
            .HasColumnName(string.Concat(prefix, nameof(PhoneNumber).ToSnakeCase(), "_", nameof(PhoneNumber.Value).ToSnakeCase()))
            .IsRequired()
            .HasMaxLength(30);
        
        builder.Property(p => p.CountryId)
            .HasColumnName(string.Concat(prefix, nameof(PhoneNumber).ToSnakeCase(), "_", nameof(PhoneNumber.CountryId).ToSnakeCase()))
            .IsRequired()
            .HasMaxLength(10)
            .HasConversion(new CountryIdValueConverter());
        
        builder.Property(p => p.CountryCode)
            .HasColumnName(string.Concat(prefix, nameof(PhoneNumber).ToSnakeCase(), "_", nameof(PhoneNumber.CountryCode).ToSnakeCase()))
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(p => p.Description)
            .HasColumnName(string.Concat(prefix, nameof(PhoneNumber).ToSnakeCase(), "_", nameof(PhoneNumber.Description).ToSnakeCase()))
            .HasMaxLength(50);
    }
}