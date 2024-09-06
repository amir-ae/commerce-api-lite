using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace Commerce.API.Extensions.ErrorHandling;

public static class ErrorEndpoint
{
    public static void AddErrorEndpoint(this WebApplication app)
    {
        app.UseExceptionHandler("/error");
        
        app.Map("/error", (HttpContext httpContext) =>
        {
            Exception? exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (!string.IsNullOrWhiteSpace(exception?.Message))
            {
                Log.Error(exception.Message);
            }
            
            return TypedResults.Problem(title: exception?.Message);
        });
    }
}