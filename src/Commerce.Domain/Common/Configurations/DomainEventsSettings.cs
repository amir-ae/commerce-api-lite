namespace Commerce.Domain.Common.Configurations;

public class DomainEventsSettings
{
    public const string Key = "DomainEventsKey";
    
    public required string DomainEventsKey { get; set; }
}