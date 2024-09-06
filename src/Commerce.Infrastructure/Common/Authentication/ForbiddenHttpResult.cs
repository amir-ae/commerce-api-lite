using Microsoft.AspNetCore.Http;

namespace Commerce.Infrastructure.Common.Authentication;

public sealed class ForbiddenHttpResult : IResult, IStatusCodeHttpResult
{
    private readonly object _body;

    public ForbiddenHttpResult(object body)
    {
        _body = body;
    }

    public int StatusCode => StatusCodes.Status403Forbidden;

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = StatusCode;
        if (_body is string s)
        {
            await httpContext.Response.WriteAsync(s);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(_body);
    }
}