namespace Commerce.Domain.Common.Configurations;

public class EventBusSettings
{
    public const string Key = "EventBus";
    
    public string ServiceUrl { get; set; } = default!;
    public string AuthenticationRegion { get; set; } = default!;
    public string KeyId { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string CustomerQueue { get; set; } = default!;
    public string ProductQueue { get; set; } = default!;
    public string ErrorQueue { get; set; } = default!;
}