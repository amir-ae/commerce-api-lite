namespace Commerce.Domain.Customers.Helpers;

public static class NameHelper
{
    public static (string FullName, string FirstName, string? MiddleName, string LastName) BuildCustomerName(
        string firstName, string? middleName, string lastName, string? fullName = null)
    {
        if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
        if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
        
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return (fullName, firstName, middleName, lastName);
        }

        var initials = firstName is [_, '.'] && middleName is [_, '.'];
        var computedFullName = string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();

        return (computedFullName, firstName, middleName, lastName);
    }

    public static string? BuildCustomerFullName(
        string? firstName, string? middleName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return null;
        }
        
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return string.Concat(firstName, " ", middleName).Trim();
        }

        return BuildCustomerName(firstName, middleName, lastName).FullName;
    }
}