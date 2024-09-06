using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.SQS;
using Catalog.API.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.Configurations;
using Commerce.Domain.Common.JsonConverters;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Infrastructure.Common.Authentication;
using Commerce.Infrastructure.Common.Authentication.Policies;
using Commerce.Infrastructure.Common.Persistence;
using Commerce.Infrastructure.Common.Persistence.Configurations;
using Commerce.Infrastructure.Common.Persistence.Projections;
using Commerce.Infrastructure.Common.Persistence.Repositories;
using Commerce.Infrastructure.Common.Services;
using Commerce.Infrastructure.Customers.Repositories;
using Commerce.Infrastructure.Products.Repositories;
using Logistics.API.Client;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Weasel.Core;

namespace Commerce.Infrastructure;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAuth(config)
            .AddCaching(config)
            .AddPersistence(config);
        
        services.AddSingleton<ILookupService, LookupService>();
        services.AddSingleton<IEnrichmentService, EnrichmentService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpClient<ICatalogClient>()
            //.AddPolicyHandler(HttpClientPolicies.RetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.CircuitBreakerPolicy())
            .AddTypedClient<ICatalogClient>((hc, sp) 
                => new CatalogClient(hc.Configure(ApiEndpoints.CatalogApi, config, sp)));
        services.AddHttpClient<ILogisticsClient>()
            //.AddPolicyHandler(HttpClientPolicies.RetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.CircuitBreakerPolicy())
            .AddTypedClient<ILogisticsClient>((hc, sp) 
                => new LogisticsClient(hc.Configure(ApiEndpoints.LogisticsApi, config, sp)));

        services.Configure<DomainEventsSettings>(config);
        services.Configure<EventBusSettings>(config.GetSection(EventBusSettings.Key));
        services.Configure<QueueSettings>(config.GetSection(QueueSettings.Key));
        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
        services.AddSingleton<ISqsMessenger, SqsMessenger>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICachingService, CachingService>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Polar";
        });
        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var serializer = new Marten.Services.JsonNetSerializer();
        serializer.Configure(c =>
        {
            c.Converters.Add(new SmartEnumJsonConverter<CustomerRole>());
            c.Converters.Add(new StronglyTypedIdJsonConverter());
            c.ContractResolver = new ResolvePrivateSetters();
        });

        var defaultConnection = configuration.GetConnectionString("Default")!;
        AWSConfigs.AWSRegion = "eu-north-1";

        services.AddScoped<ICustomerRepository, CustomerMartenRepository>();
        services.AddScoped<IProductRepository, ProductMartenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventSubscriptionManager, EventSubscriptionManager>();
        
        services.AddDbContextPool<CommerceDbContext>((serviceProvider, contextOptions) =>
        {
            contextOptions.UseNpgsql(defaultConnection, serverOptions =>
            {
                serverOptions.MigrationsAssembly(typeof(CommerceDbContext).Assembly.FullName);
                serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
        
        services.AddMarten(storeOptions =>
        {
            var schemaName = Environment.GetEnvironmentVariable("SchemaName") ?? "commerce";
            storeOptions.DatabaseSchemaName = schemaName;
            storeOptions.Connection(defaultConnection);
            
            storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOnly;
            storeOptions.Serializer(serializer);
            storeOptions.ConfigurePolly(o => o.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 2,
            }));

            /*var maintenanceConnection = configuration.GetConnectionString("Maintenance")!;
            storeOptions.CreateDatabasesForTenants(c =>
            {
                c.MaintenanceDatabase(maintenanceConnection);
                c.ForTenant()
                    .CheckAgainstPgDatabase()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1);
            });*/
            
            var stuff = Assembly.GetExecutingAssembly()
                .DefinedTypes.Where(type => type.IsSubclassOf(typeof(MartenTableMetaDataBase))).ToList();

            foreach (Type type in stuff)
            {
                IMartenTableMetaData temp = (IMartenTableMetaData)Activator.CreateInstance(type)!;
                temp.SetTableMetaData(storeOptions);
            }
        })
            //.ApplyAllDatabaseChangesOnStartup()
            .AddProjectionWithServices<FlatTablesProjection>(ProjectionLifecycle.Inline, ServiceLifetime.Singleton)
            .UseLightweightSessions()
            .AddAsyncDaemon(DaemonMode.Solo);
        
        services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            });

        services.AddAuthorization();

        return services;
    }
    
    private static HttpClient Configure(this HttpClient hc, string baseAddress, IConfiguration config, IServiceProvider sp)
    {
        hc.BaseAddress = new Uri(config.GetValue<string>(baseAddress)!);
        hc.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName, config.GetValue<string>(ApiKeyConstants.SectionName));
        var request = sp.GetService<IHttpContextAccessor>()?.HttpContext?.Request;
        if (request is not null)
        {
            _ = AuthenticationHeaderValue.TryParse(request.Headers.Authorization, out var headerValue);
            hc.DefaultRequestHeaders.Authorization = headerValue;
        }
        return hc;
    }
}