using System.Text;
using System.Text.Json.Serialization;

namespace Commerce.Domain.Customers.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; init; }
    public CountryId CountryId { get; init; }
    public string CountryCode { get; init; }
    public string? Description { get; init; }
    [JsonIgnore]
    public string FullNumber => string.Concat(CountryCode, " ", Value).Trim();

    public PhoneNumber(
        string value,
        CountryId? countryId = null,
        string? countryCode = null,
        string? description = null)
    {
        Description = description;
        CountryId = countryId ?? new CountryId("RU");

        switch (CountryId.Value)
        {
            case "RU":
                CountryCode = "+7";
                break;
            case "BY":
                CountryCode = "+375";
                break;
            case "UA":
                CountryCode = "+380";
                break;
            case "ru":
            case "Russia":
            case "Россия":
            case "+7":
            case "7":
            case "8":
                CountryId = new CountryId("RU");
                CountryCode = "+7";
                break;
            case "by":
            case "Belarus":
            case "Беларусь":
            case "+375":
            case "375":
                CountryId = new CountryId("BY");
                CountryCode = "+375";
                break;
            case "ua":
            case "Ukraine":
            case "Украина":
            case "+380":
            case "380":
                CountryId = new CountryId("UA");
                CountryCode = "+380";
                break;
            default:
                CountryCode = countryCode ?? string.Empty;
                break;
        }

        switch (CountryId.Value)
        {
            case "RU":
                if (value.Length == 15 
                    && value.Contains('(') && value.Contains(')') 
                    && value.Count(Char.IsWhiteSpace) == 3)
                {
                    Value = value;
                    return;
                }
            
                var sb1 = new StringBuilder(new string(value.Where(char.IsDigit).ToArray()));
                if (sb1.Length == 11 && (sb1[0] == '7' || sb1[0] == '8'))
                {
                    sb1.Remove(0, 1);
                }
                if (sb1.Length == 10)
                {
                    sb1.Insert(8, ' ');
                    sb1.Insert(6, ' ');
                    sb1.Insert(3, ") ");
                    sb1.Insert(0, '(');
                }
                
                Value = sb1.ToString();
                break;
            case "BY":
            case "UA":
                if (value.Length == 14 
                    && value.Contains('(') && value.Contains(')') 
                    && value.Count(Char.IsWhiteSpace) == 3)
                {
                    Value = value;
                    return;
                }

                var sb2 = new StringBuilder(new string(value.Where(char.IsDigit).ToArray()));
                if (sb2.Length == 12)
                {
                    switch (sb2.ToString(0, 3))
                    {
                        case "375":
                        case "380":
                            sb2.Remove(0, 3);
                            break;
                    }
                }
            
                if (sb2.Length == 9)
                {
                    sb2.Insert(7, ' ');
                    sb2.Insert(5, ' ');
                    sb2.Insert(2, ") ");
                    sb2.Insert(0, '(');
                }
                
                Value = sb2.ToString();
                break;
            default:
                Value = value;
                break;
        }
    }
    
    public async ValueTask<PhoneNumber> InspectCountryCode(Func<CountryId, CancellationToken, ValueTask<string?>> countryCode, 
        CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(CountryCode)) return this;

        return this with
        {
            CountryCode = await countryCode(CountryId, ct) ?? string.Empty
        };
    }

#pragma warning disable CS8618
    private PhoneNumber() { }
#pragma warning restore CS8618
}
