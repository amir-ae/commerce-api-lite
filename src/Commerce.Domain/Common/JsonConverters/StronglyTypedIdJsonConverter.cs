using System.Collections.Concurrent;
using Commerce.Domain.Common.Models;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using Newtonsoft.Json;

namespace Commerce.Domain.Common.JsonConverters;

public class StronglyTypedIdJsonConverter : JsonConverter
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> Cache = new();
    private static readonly List<string> ConvertNotStronglyTypedIds = new()
    {
        nameof(ProductId), nameof(CustomerId), nameof(OrderId), nameof(CentreId)
    };

    public override bool CanConvert(Type objectType)
    {
        return StronglyTypedIdHelper.IsStronglyTypedId(objectType)
            && !ConvertNotStronglyTypedIds.Contains(objectType.Name);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var converter = Converter(objectType);
        return converter.ReadJson(reader, objectType, existingValue, serializer)!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            var converter = Converter(value.GetType());
            converter.WriteJson(writer, value, serializer);
        }
    }

    private static JsonConverter Converter(Type objectType)
    {
        return Cache.GetOrAdd(objectType, CreateConverter);
    }

    private static JsonConverter CreateConverter(Type objectType)
    {
        if (!StronglyTypedIdHelper.IsStronglyTypedId(objectType, out var valueType))
            throw new InvalidOperationException($"Cannot create converter for '{objectType}'");

        var type = typeof(StronglyTypedIdNewtonsoftJsonConverter<,>).MakeGenericType(objectType, valueType);
        return (JsonConverter)Activator.CreateInstance(type)!;
    }
}

public class StronglyTypedIdNewtonsoftJsonConverter<TStronglyTypedId, TValue> : JsonConverter<TStronglyTypedId>
    where TStronglyTypedId : StronglyTypedId<TValue>
    where TValue : IComparable<TValue>
{
    public override TStronglyTypedId? ReadJson(JsonReader reader, Type objectType, TStronglyTypedId? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Null)
            return null;

        var value = serializer.Deserialize<TValue>(reader);
        var factory = StronglyTypedIdHelper.Factory<TValue>(objectType);
        return (TStronglyTypedId)factory(value!);
    }

    public override void WriteJson(JsonWriter writer, TStronglyTypedId? value, JsonSerializer serializer)
    {
        if (value is null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value);
    }
}