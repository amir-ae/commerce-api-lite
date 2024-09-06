using Commerce.API.Contract.V1;
using StackExchange.Redis;

namespace Commerce.API.Extensions.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost"));
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Customers.List.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Customers.ById.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Customers.DetailById.Uri()))
                .Tag(nameof(Routes.Customers)).Expire(TimeSpan.FromSeconds(30)));
            options.AddBasePolicy(builder => builder
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Products.List.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Products.ById.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Products.DetailById.Uri()))
                .Tag(nameof(Routes.Products)).Expire(TimeSpan.FromSeconds(30)));
            options.AddBasePolicy(builder => builder
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Orders.List.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Orders.ById.Uri()))
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Orders.DetailById.Uri()))
                .Tag(nameof(Routes.Orders)).Expire(TimeSpan.FromSeconds(30)));
            options.AddPolicy("Auth", CacheAuthenticatedRequestsPolicy.Instance);
        });
        return services;
    }
}