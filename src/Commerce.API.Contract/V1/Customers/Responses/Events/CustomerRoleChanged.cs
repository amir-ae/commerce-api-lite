using Commerce.API.Contract.V1.Common.Models;

namespace Commerce.API.Contract.V1.Customers.Responses.Events;

public record CustomerRoleChanged(
    CustomerRole Role,
    DateTimeOffset RoleChangedAt) : CustomerEvent;