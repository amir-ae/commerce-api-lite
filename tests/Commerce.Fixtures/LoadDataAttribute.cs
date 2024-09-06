using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using Commerce.Domain.Common.JsonConverters;
using JsonNet.ContractResolvers;
using Xunit.Sdk;
using CustomerRole = Commerce.Domain.Customers.ValueObjects.CustomerRole;

namespace Commerce.Fixtures;

public class LoadDataAttribute : DataAttribute
{
    private readonly string _fileName;
    private readonly string _section;
    public LoadDataAttribute(string section)
    {
        _fileName = "./Data/record-data.json";
        _section = section;
    }
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null) throw new ArgumentNullException(nameof(testMethod));

        var path = Path.IsPathRooted(_fileName)
            ? _fileName
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _fileName);

        if (!File.Exists(path)) throw new ArgumentException($"File not found: {path}");

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver(),
            Converters =
            {
                new SmartEnumJsonConverter<CustomerRole>(),
            }
        };

        var fileData = File.ReadAllText(_fileName);

        if (string.IsNullOrWhiteSpace(_section)) return
            JsonConvert.DeserializeObject<List<string[]>>(fileData, settings)!;

        var allData = JToken.Parse(fileData);
        var query = _section.Trim('/').Replace('/', '.');
            
        var data = allData.SelectToken(query);

        return new List<object[]>
        {
            new[] { data!.ToObject(testMethod.GetParameters().First().ParameterType, 
                JsonSerializer.Create(settings)) }!
        };
    }
}