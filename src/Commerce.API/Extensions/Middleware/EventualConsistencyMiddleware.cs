using Commerce.API.Extensions.ErrorHandling;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Domain.Common.Configurations;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Commerce.API.Extensions.Middleware;

public class EventualConsistencyMiddleware
{
    private readonly RequestDelegate _next;

    public EventualConsistencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, IUnitOfWork unitOfWork, 
        IOptions<DomainEventsSettings> options, IErrorHandler errorHandler)
    {
        if (context.Request.Method != HttpMethods.Get)
        {
            context.Response.OnStarting(async () =>
            {
                try
                {
                    if (context.Response.StatusCode >= 400)
                    {
                        return;
                    }
                    
                    if (context.Items.TryGetValue(options.Value.DomainEventsKey, out var value) 
                        && value is Queue<IDomainEvent> domainEvents)
                    {
                        while (domainEvents.TryDequeue(out var nextEvent))
                        {
                            await publisher.Publish(nextEvent);
                        }
                    }

                    await unitOfWork.SaveChangesAsync();
                }
                catch (EventualConsistencyException ex)
                {
                    var result = errorHandler.Problem(ex.UnderlyingErrors);

                    if (!context.Response.HasStarted)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "application/json";

                        await result.ExecuteAsync(context);
                    }
                }
            });
        }

        await _next(context);
    }
}