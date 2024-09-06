namespace Commerce.API.Extensions.Swagger;

public class SwaggerSchemaHelper
{
    private readonly Dictionary<string, int> _schemaNameRepetition = new();
    private const string PrefixBoundary = "V1";
    
    private string DefaultSchemaId(Type modelType)
    {
        if (!modelType.IsConstructedGenericType) return modelType.Name.Replace("[]", "Array");

        var prefix = modelType.GetGenericArguments()
            .Select(DefaultSchemaId)
            .Aggregate((previous, current) => previous + current);

        return prefix + modelType.Name.Split('`').First();
    }

    private string SchemaIdPrefix(string modelNamespace)
    {
        var segments = modelNamespace.Split(".").ToArray();
        var boundaryIndex = Array.IndexOf(segments, PrefixBoundary);
        return string.Join(".", segments.Skip(boundaryIndex + 1));
    }

    public string SchemaId(Type modelType)
    {
        string id = DefaultSchemaId(modelType);

        _schemaNameRepetition.TryAdd(id, 0);
        int count = _schemaNameRepetition[id] + 1;
        _schemaNameRepetition[id] = count;
        
        if (string.IsNullOrWhiteSpace(modelType.Namespace))
        {
            return $"{id}{(count > 1 ? count.ToString() : "")}";
        }
        
        var prefix = SchemaIdPrefix(modelType.Namespace);
        return string.Concat(prefix, ".", id);
    }
}