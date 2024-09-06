namespace Commerce.Domain.Common.Configurations;

public class QueueSettings
{
    public const string Key = "Queues";
    
    public required string Customer { get; set; }
    public required string Product { get; set; }

    public object? this[string name]
    {
        get
        {
            var enumerable = GetType().GetProperties().Select(x => x.Name);
            var properties = enumerable as string[] ?? enumerable.ToArray();

            var prop = GetType().GetProperty(Examine(name, properties) ?? name);

            if(prop is not null)
                return prop.GetValue(this, null);

            return null; 
        }
    }

    private static string? Examine(string name, string[] properties)
    {
        var equals = CheckEquals(name, properties);
        if (!string.IsNullOrWhiteSpace(equals)) return equals;
        
        var startsWith = CheckStartsWith(name, properties);
        if (!string.IsNullOrWhiteSpace(startsWith)) return startsWith;
        
        var contains = CheckContains(name, properties);
        if (!string.IsNullOrWhiteSpace(contains)) return contains;

        return null;
    }

    private static readonly Func<string, IEnumerable<string>, string?> CheckEquals = (name, properties) =>
    {
        foreach (var propertyName in properties)
        {
            if (name == propertyName) return propertyName;
        }
        return null;
    };
    
    private static readonly Func<string, IEnumerable<string>, string?> CheckStartsWith = (name, properties) =>
    {
        foreach (var propertyName in properties)
        {
            if (name.StartsWith(propertyName)) return propertyName;
        }
        return null;
    };
    
    private static readonly Func<string, IEnumerable<string>, string?> CheckContains = (name, properties) =>
    {
        foreach (var propertyName in properties)
        {
            if (name.Contains(propertyName)) return propertyName;
        }
        return null;
    };
}
