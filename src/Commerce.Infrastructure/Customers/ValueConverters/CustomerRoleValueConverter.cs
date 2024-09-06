using Ardalis.SmartEnum;
using Commerce.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.Infrastructure.Customers.ValueConverters;

public class CustomerRoleValueConverter : ValueConverter<CustomerRole, int>
{
    public CustomerRoleValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            role => role.Value,
            value => SmartEnum<CustomerRole>.FromValue(value),
            mappingHints
        ) { }
}