using Commerce.API.Contract.V1.Common.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using static Commerce.API.Extensions.ETag.ETagExtensions;

namespace Commerce.API.Extensions.ETag;

public class ETagEndpointFilter<T, TU> : IEndpointFilter 
    where T : AuditableResponse 
    where TU : BriefResponse
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        
        var response = await next(context);
        
        if (request.Method != HttpMethod.Get.Method) return response;

        int? version = response switch
        {
            Ok<T> e => e.Value?.Version,
            Ok<PaginatedList<T>> l => l.Value?.Data.GetCombinedHashCode(),
            Ok<List<T>> l => l.Value?.GetCombinedHashCode(),
            Ok<T[]> l => l.Value?.GetCombinedHashCode(),
            Ok<List<TU>> l => l.Value?.GetCombinedHashCode(),
            Ok<TU[]> l => l.Value?.GetCombinedHashCode(),
            _ => null
        };

        if (!version.HasValue) return response;

        var eTag = new EntityTagHeaderValue('\"' + version.Value.ToString() + '\"');
        context.HttpContext.Response.Headers.TryAdd(HeaderNames.ETag, eTag.ToString());
            
        if (request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var ifNoneMatch))
        {
            var headerValue = ToVersion(ifNoneMatch);
            if (headerValue.HasValue && headerValue.Equals(version))
            {
                return new StatusCodeResult(StatusCodes.Status304NotModified);
            }
        }

        return response;
    }
}