using Commerce.API.Extensions.ErrorHandling;
using Commerce.API.Extensions.Swagger;
using Microsoft.Extensions.Options;
using Commerce.API.Extensions.Caching;
using Commerce.API.Extensions.Mapping;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Commerce.API;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var schemaHelper = new SwaggerSchemaHelper();
            options.CustomSchemaIds(type => schemaHelper.SchemaId(type));
        });
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSingleton<IErrorHandler, ErrorHandler>();
        services.AddMappings();
        services.AddCaching();
        return services;
    }
}