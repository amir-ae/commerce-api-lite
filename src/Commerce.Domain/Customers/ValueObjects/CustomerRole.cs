using Ardalis.SmartEnum;

namespace Commerce.Domain.Customers.ValueObjects;

public sealed class CustomerRole : SmartEnum<CustomerRole>
{
    public static readonly CustomerRole Owner = new(nameof(Owner), 1);
    public static readonly CustomerRole Dealer = new(nameof(Dealer), 8);
    public CustomerRole(string name, int value) : base(name, value)
    {
    }
}