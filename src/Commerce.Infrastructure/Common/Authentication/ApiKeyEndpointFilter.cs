using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Commerce.Infrastructure.Common.Authentication;

public class ApiKeyEndpointFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyEndpointFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out
                var extractedApiKey))
        {
            return new UnauthorizedHttpResult("API Key missing");
        }
        
        var apiKey = _configuration.GetValue<string>(ApiKeyConstants.SectionName)!;

        if (!apiKey.Equals(extractedApiKey))
        {
            return new ForbiddenHttpResult("Invalid API Key");
        };

        return await next(context);
    }
}