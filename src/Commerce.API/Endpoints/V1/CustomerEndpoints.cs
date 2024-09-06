using System.Collections.Concurrent;
using Commerce.API.Extensions.ErrorHandling;
using Commerce.API.Extensions.ETag;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Requests;
using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.Application.Customers.Commands.Create;
using Commerce.Application.Customers.Commands.Delete;
using Commerce.Application.Customers.Commands.Update;
using Commerce.Application.Customers.Commands.Activate;
using Commerce.Application.Customers.Commands.Deactivate;
using Commerce.Application.Customers.Queries.List;
using Commerce.Application.Customers.Queries.ListDetail;
using Commerce.Application.Customers.Queries.ByPage;
using Commerce.Application.Customers.Queries.ById;
using Commerce.Application.Customers.Queries.DetailById;
using Commerce.Application.Customers.Queries.EventsById;
using Commerce.Application.Customers.Queries.ByPageDetail;
using Commerce.Application.Products.Commands.Upsert;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Customers = Commerce.API.Contract.V1.Routes.Customers;
using Products = Commerce.API.Contract.V1.Routes.Products;
using ProductRequest = Commerce.API.Contract.V1.Common.Requests.Product;
using static Commerce.API.Extensions.ETag.ETagExtensions;
using ErrorOr;
using Customer = Commerce.Domain.Customers.Customer;

namespace Commerce.API.Endpoints.V1;

public static class CustomerEndpoints
{
    public static void AddCustomerEndpoints(this WebApplication app)
    {
        var customersGroup = app.MapGroup(Customers.Prefix)
            
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .AddEndpointFilter<ETagEndpointFilter<CustomerResponse, CustomerBriefResponse>>()
            .WithTags(nameof(Customer))
            .WithOpenApi();
        
        customersGroup.MapGet(Customers.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageNumber, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageQuery(pageSize ?? 10, pageNumber ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null,
                        !string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ByPage.Name)
            .WithDescription(Customers.ByPage.Description);

        customersGroup.MapGet(Customers.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageNumber, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageDetailQuery(pageSize ?? 10, pageNumber ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null,
                        !string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ByPageDetail.Name)
            .WithDescription(Customers.ByPageDetail.Description);

        customersGroup.MapGet(Customers.List.Pattern,
                async ([FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListCustomersQuery(!string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<CustomerBriefResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.List.Name)
            .WithDescription(Customers.List.Description)
            .CacheOutput("Auth");
        
        customersGroup.MapGet(Customers.ListDetail.Pattern, 
                async ([FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListCustomersDetailQuery(!string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ListDetail.Name)
            .WithDescription(Customers.ListDetail.Description);
        
        customersGroup.MapGet(Customers.ById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new CustomerByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ById.Name)
            .WithDescription(Customers.ById.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.DetailById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerDetailByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.DetailById.Name)
            .WithDescription(Customers.DetailById.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.EventsById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerEventsByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerEventsResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.EventsById.Name)
            .WithDescription(Customers.EventsById.Description);

        customersGroup.MapPost(Customers.Create.Pattern,
                async ([FromBody] CreateCustomerRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<CreateCustomerCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            var productResults = await UpsertProducts(
                                request.Products?.ToHashSet(), customer, request.CreateBy, request.CreateAt, mediator, ct);
                            if (productResults.Any(p => p is { IsError: true }))
                            {
                                return errorHandler.Problem(productResults.First(p => p is { IsError: true }).Errors);
                            }
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Created(new Uri(Customers.ById.Uri(customer.Id.Value), 
                                UriKind.Relative), customer.Adapt<CustomerResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Create.Name)
            .WithDescription(Customers.Create.Description);

        customersGroup.MapPatch(Customers.Update.Pattern,
                async ([FromRoute] string customerId, [FromHeader(Name = "If-Match")] string? eTag,
                    [FromBody] UpdateCustomerRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<UpdateCustomerCommand>() with
                    {
                        CustomerId = new CustomerId(customerId), Version = ToVersion(eTag)
                    };
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            var productResults = await UpsertProducts(
                                request.Products?.ToHashSet(), customer, request.UpdateBy, request.UpdateAt, mediator, ct);
                            if (productResults.Any(p => p is { IsError: true }))
                            {
                                return errorHandler.Problem(productResults.First(p => p is { IsError: true }).Errors);
                            }
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Ok(customer.Adapt<CustomerResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status412PreconditionFailed)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Update.Name)
            .WithDescription(Customers.Update.Description);

        customersGroup.MapPatch(Customers.Activate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid activateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateCustomerCommand(new CustomerId(customerId), new AppUserId(activateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer.Adapt<CustomerResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Activate.Name)
            .WithDescription(Customers.Activate.Description);

        customersGroup.MapPatch(Customers.Deactivate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid deactivateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateCustomerCommand(new CustomerId(customerId), new AppUserId(deactivateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer.Adapt<CustomerResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Deactivate.Name)
            .WithDescription(Customers.Deactivate.Description);

        customersGroup.MapDelete(Customers.Delete.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid deleteBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteCustomerCommand(new CustomerId(customerId), new AppUserId(deleteBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.NoContent();
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Delete.Name)
            .WithDescription(Customers.Delete.Description);
        
        async Task<HashSet<ErrorOr<Product>>> UpsertProducts(
            HashSet<ProductRequest>? productRequests, Customer customer, Guid upsertBy, DateTimeOffset? upsertAt, 
            ISender mediator, CancellationToken ct)
        {
            if (productRequests == null || productRequests.Count == 0) 
                return new HashSet<ErrorOr<Product>>();

            var productResults = new ConcurrentBag<ErrorOr<Product>>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = ct
            };
            
            await Parallel.ForEachAsync(productRequests, parallelOptions, async (productRequest, token) =>
            {
                var roleIsOwner = customer.Role == CustomerRole.Owner;
                var upsertCommand = productRequest.Adapt<UpsertProductCommand>() with
                {
                    OwnerId = roleIsOwner ? customer.Id : null,
                    DealerId = !roleIsOwner ? customer.Id : null,
                    UpsertBy = new AppUserId(upsertBy),
                    UpsertAt = upsertAt
                };

                var productResult = await mediator.Send(upsertCommand, token).DefaultIfCanceled();
                productResults.Add(productResult);
            });

            return productResults.ToHashSet();
        }
    }
}