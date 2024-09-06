using Commerce.API.Contract.V1.Customers.Responses.Models;

namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerPhoneNumberChanged(
    PhoneNumber PhoneNumber,
    DateTimeOffset PhoneNumberChangedAt) : CustomerEvent;