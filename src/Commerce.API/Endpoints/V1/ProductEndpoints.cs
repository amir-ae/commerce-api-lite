using Commerce.API.Extensions.ErrorHandling;
using Commerce.API.Extensions.ETag;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Products.Requests;
using Commerce.API.Contract.V1.Products.Responses;
using Commerce.Application.Customers.Commands.Upsert;
using Commerce.Application.Products.Commands.Create;
using Commerce.Application.Products.Commands.Delete;
using Commerce.Application.Products.Commands.Update;
using Commerce.Application.Products.Commands.Activate;
using Commerce.Application.Products.Commands.Deactivate;
using Commerce.Application.Products.Queries.List;
using Commerce.Application.Products.Queries.ListDetail;
using Commerce.Application.Products.Queries.ByPage;
using Commerce.Application.Products.Queries.ById;
using Commerce.Application.Products.Queries.DetailById;
using Commerce.Application.Products.Queries.DetailByOrderId;
using Commerce.Application.Products.Queries.EventsById;
using Commerce.Application.Products.Queries.ByPageDetail;
using Commerce.Application.Products.Queries.CheckById;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Products;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Products = Commerce.API.Contract.V1.Routes.Products;
using Customers = Commerce.API.Contract.V1.Routes.Customers;
using CustomerRequest = Commerce.API.Contract.V1.Common.Requests.Customer;
using static Commerce.API.Extensions.ETag.ETagExtensions;
using ErrorOr;
using Customer = Commerce.Domain.Customers.Customer;
using OrderId = Commerce.Domain.Common.ValueObjects.OrderId;

namespace Commerce.API.Endpoints.V1;

public static class ProductEndpoints
{
    public static void AddProductEndpoints(this WebApplication app)
    {
        var productsGroup = app.MapGroup(Products.Prefix)
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .AddEndpointFilter<ETagEndpointFilter<ProductResponse, ProductBriefResponse>>()
            .WithTags(nameof(Product))
            .WithOpenApi();

        productsGroup.MapGet(Products.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageNumber, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageQuery(pageSize ?? 10, pageNumber ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null,
                        !string.IsNullOrWhiteSpace(centreId)? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ByPage.Name)
            .WithDescription(Products.ByPage.Description);

        productsGroup.MapGet(Products.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageNumber, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageDetailQuery(pageSize ?? 10, pageNumber ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null,
                        !string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ByPageDetail.Name)
            .WithDescription(Products.ByPageDetail.Description);

        productsGroup.MapGet(Products.List.Pattern,
                async ([FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListProductsQuery(!string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<ProductBriefResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.List.Name)
            .WithDescription(Products.List.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ListDetail.Pattern,
                async ([FromQuery] string? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListProductsDetailQuery(!string.IsNullOrWhiteSpace(centreId) ? new CentreId(centreId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ListDetail.Name)
            .WithDescription(Products.ListDetail.Description);

        productsGroup.MapGet(Products.ById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ById.Name)
            .WithDescription(Products.ById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.DetailById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductDetailByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.DetailById.Name)
            .WithDescription(Products.DetailById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.EventsById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductEventsByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductEventsResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.EventsById.Name)
            .WithDescription(Products.EventsById.Description);
        
        productsGroup.MapGet(Products.Check.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CheckProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<bool>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Check.Name)
            .WithDescription(Products.Check.Description);

        productsGroup.MapGet(Products.DetailByOrderId.Pattern,
                async ([FromRoute] string orderId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new ProductDetailByOrderIdQuery(new OrderId(orderId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.DetailByOrderId.Name)
            .WithDescription(Products.DetailByOrderId.Description);

        productsGroup.MapPost(Products.Create.Pattern,
                async ([FromBody] CreateProductRequest request, ISender mediator, IOutputCacheStore cache,
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var productCommand = request.Adapt<CreateProductCommand>();
                    var productTask = mediator.Send(productCommand, ct).DefaultIfCanceled();
                    var customersTask = UpsertCustomers(request.Owner, request.Dealer, request.ProductId, 
                        request.CreateBy, request.CreateAt, mediator, ct);
                    
                    await Task.WhenAll(productTask, customersTask);
                    
                    var result = await productTask;
                    return await result.MatchAsync(async product =>
                        {
                            var (ownerResult, dealerResult) = await customersTask;
                            if (ownerResult is { IsError: true }) return errorHandler.Problem(ownerResult.Value.Errors);
                            if (dealerResult is { IsError: true }) return errorHandler.Problem(dealerResult.Value.Errors);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Created(new Uri(Products.ById.Uri(product.Id.Value), UriKind.Relative),
                                product.Adapt<ProductResponse>());
                        }, 
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Create.Name)
            .WithDescription(Products.Create.Description);

        productsGroup.MapPatch(Products.Update.Pattern,
                async ([FromRoute] string productId, [FromHeader(Name = "If-Match")] string? eTag, 
                    [FromBody] UpdateProductRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var productCommand = request.Adapt<UpdateProductCommand>() with
                    {
                        ProductId = new ProductId(productId),
                        OrderIds = request.Orders?.ToList().ConvertAll(o => new OrderId(o.OrderId, o.CentreId)).ToHashSet(),
                        Version = ToVersion(eTag)
                    };
                    var productTask = mediator.Send(productCommand, ct).DefaultIfCanceled();
                    var customersTask = UpsertCustomers(request.Owner, request.Dealer, productId, 
                        request.UpdateBy, request.UpdateAt, mediator, ct);
                    
                    await Task.WhenAll(productTask, customersTask);
                    
                    var result = await productTask;
                    return await result.MatchAsync(async product =>
                        {
                            var (ownerResult, dealerResult) = await customersTask;
                            if (ownerResult is { IsError: true }) return errorHandler.Problem(ownerResult.Value.Errors);
                            if (dealerResult is { IsError: true }) return errorHandler.Problem(dealerResult.Value.Errors);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Ok(product.Adapt<ProductResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status412PreconditionFailed)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Update.Name)
            .WithDescription(Products.Update.Description);

        productsGroup.MapPatch(Products.Activate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid activateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateProductCommand(new ProductId(productId), new AppUserId(activateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product.Adapt<ProductResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Activate.Name)
            .WithDescription(Products.Activate.Description);

        productsGroup.MapPatch(Products.Deactivate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid deactivateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateProductCommand(new ProductId(productId), new AppUserId(deactivateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product.Adapt<ProductResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Deactivate.Name)
            .WithDescription(Products.Deactivate.Description);

        productsGroup.MapDelete(Products.Delete.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid deleteBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteProductCommand(new ProductId(productId), new AppUserId(deleteBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
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
            .WithName(Products.Delete.Name)
            .WithDescription(Products.Delete.Description);
        
        async Task<(ErrorOr<Customer>? Owner, ErrorOr<Customer>? Dealer)> UpsertCustomers(
            CustomerRequest? ownerRequest, CustomerRequest? dealerRequest, string productId, Guid upsertBy, 
            DateTimeOffset? upsertAt, ISender mediator, CancellationToken ct)
        {
            Task<ErrorOr<Customer>>? ownerTask = null;
            Task<ErrorOr<Customer>>? dealerTask = null;

            if (ownerRequest is not null)
            {
                var ownerCommand = ownerRequest.Adapt<UpsertCustomerCommand>() with
                {
                    ProductIds = new HashSet<ProductId> { new(productId) }, UpsertBy = new AppUserId(upsertBy), UpsertAt = upsertAt
                };
                ownerTask = mediator.Send(ownerCommand, ct).DefaultIfCanceled();
            }

            if (dealerRequest is not null)
            {
                var dealerCommand = dealerRequest.Adapt<UpsertCustomerCommand>() with
                {
                    ProductIds = new HashSet<ProductId> { new(productId) }, UpsertBy = new AppUserId(upsertBy), UpsertAt = upsertAt
                };
                dealerTask = mediator.Send(dealerCommand, ct).DefaultIfCanceled();
            }

            await Task.WhenAll(ownerTask ?? Task.CompletedTask, dealerTask ?? Task.CompletedTask);

            var ownerResult = ownerTask?.Result;
            var dealerResult = dealerTask?.Result;

            return (ownerResult, dealerResult);
        }
    }
}