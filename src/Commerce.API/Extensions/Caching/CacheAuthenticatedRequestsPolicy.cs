﻿using Microsoft.AspNetCore.OutputCaching;

namespace Commerce.API.Extensions.Caching;

public class CacheAuthenticatedRequestsPolicy : IOutputCachePolicy
{

    public static readonly CacheAuthenticatedRequestsPolicy Instance = new();

    private CacheAuthenticatedRequestsPolicy()
    {
    }

    ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by any query by default
        context.CacheVaryByRules.QueryKeys = "*";
        return ValueTask.CompletedTask;
    }
    // this never gets hit when Authorization is present
    ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;
        context.AllowCacheStorage = true;

        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context)
    {
        // Check if the current request fulfills the requirements to be cached

        var request = context.HttpContext.Request;

        // Verify the method
        if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
        {
            return false;
        }

        // Verify existence of authorization headers
        //if (!StringValues.IsNullOrWhiteSpace(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
        //{
        //    return false;
        //}
        return true;
    }
}