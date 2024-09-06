using Commerce.Domain.Common.JsonConverters;
using Commerce.Domain.Customers.ValueObjects;
using JsonNet.ContractResolvers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Commerce.Fixtures.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder Seed<T>(this ModelBuilder modelBuilder, string file) where T : class
    {
        using var reader = new StreamReader(file);
        var settings = new JsonSerializerSettings
        {
            Converters = { new SmartEnumJsonConverter<CustomerRole>() },
            ContractResolver = new PrivateSetterContractResolver()
        };
                
        var json = reader.ReadToEnd();
        var data = JsonConvert.DeserializeObject<T[]>(json, settings);
        modelBuilder.Entity<T>().HasData(data!);
        return modelBuilder;
    }
}