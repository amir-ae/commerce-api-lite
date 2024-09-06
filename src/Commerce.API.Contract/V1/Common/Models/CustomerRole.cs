using System.Text.Json.Serialization;

namespace Commerce.API.Contract.V1.Common.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerRole
{
    Владелец = 1,
    Owner = 1,
    Дилер = 8,
    Dealer = 8
}