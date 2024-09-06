using Commerce.Infrastructure;
using Commerce.API;
using Commerce.API.Extensions.ErrorHandling;
using Commerce.API.Endpoints.V1;
using Commerce.API.Extensions.Database;
using Commerce.API.Extensions.Middleware;
using Commerce.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, config) 
    => config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.AddErrorEndpoint();
app.UseMiddleware<EventualConsistencyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Commerce API V1");
        c.RoutePrefix = "swagger";
    });
    app.CreateDatabase();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.AddCustomerEndpoints();
app.AddProductEndpoints();

app.Run();

public partial class Program { }