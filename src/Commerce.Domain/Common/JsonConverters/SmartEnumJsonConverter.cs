using Ardalis.SmartEnum;
using Newtonsoft.Json;

namespace Commerce.Domain.Common.JsonConverters;

public class SmartEnumJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : SmartEnum<TEnum, int>
{
    public override TEnum? ReadJson(JsonReader reader, Type objectType, TEnum? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Null)
            return null;
        
        if (reader.TokenType == JsonToken.Integer)
        {
            int value = Convert.ToInt32(reader.Value);
            return SmartEnum<TEnum, int>.FromValue(value);
        }

        throw new JsonSerializationException($"Unexpected token type '{reader.TokenType}' when deserializing SmartEnum.");
    }

    public override void WriteJson(JsonWriter writer, TEnum? value, JsonSerializer serializer)
    {
        if (value is null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value);
    }
}