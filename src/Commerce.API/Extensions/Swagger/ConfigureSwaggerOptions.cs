using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Commerce.API.Extensions.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Commerce API", Version = "v1" });
        options.EnableAnnotations();
        options.AddApiKeyAuthorization();
        options.AddBearerAuthorization();
    }
}

public static class SwaggerGenOptionsExtensions
{
    public static void AddApiKeyAuthorization(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "The API Key to access the API",
            Type = SecuritySchemeType.ApiKey,
            Name = "x-api-key",
            In = ParameterLocation.Header,
            Scheme = "ApiKeyScheme"
        });
        var apiKeyScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "ApiKey",
                Type = ReferenceType.SecurityScheme
            },
            In = ParameterLocation.Header
        };
        
        var apiKeyRequirement = new OpenApiSecurityRequirement
        {
            { apiKeyScheme, new List<string>() }
        };
        options.AddSecurityRequirement(apiKeyRequirement);
    }
    
    public static void AddBearerAuthorization(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
        };
        var bearerRequirement = new OpenApiSecurityRequirement
        {
            { bearerScheme, new List<string>() }
        };
        options.AddSecurityRequirement(bearerRequirement);
    }
}