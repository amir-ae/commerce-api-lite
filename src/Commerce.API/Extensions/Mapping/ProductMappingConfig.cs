using Commerce.API.Contract.V1.Products.Requests;
using Mapster;
using Commerce.API.Contract.V1.Products.Responses;
using Commerce.API.Contract.V1.Products.Responses.Events;
using Commerce.Application.Products.Commands.Create;
using Commerce.Application.Products.Commands.Update;
using Commerce.Application.Products.Commands.Upsert;
using Commerce.Domain.Products.Events;
using CustomerAggregate = Commerce.Domain.Customers.Customer;
using ProductAggregate = Commerce.Domain.Products.Product;

namespace Commerce.API.Extensions.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductRequest, CreateProductCommand>()
            .Map(command => command.OwnerId, request => request.Owner == null ? null : request.Owner.CustomerId)
            .Map(command => command.DealerId, request => request.Dealer == null ? null : request.Dealer.CustomerId)
            .Map(command => command.OrderIds, request => request.Orders);

        config.NewConfig<UpdateProductRequest, UpdateProductCommand>()
            .Map(command => command.OwnerId, request => request.Owner == null ? null : request.Owner.CustomerId)
            .Map(command => command.DealerId, request => request.Dealer == null ? null : request.Dealer.CustomerId)
            .Map(command => command.OrderIds, request => request.Orders);

        config.NewConfig<UpsertProductCommand, CreateProductCommand>()
            .Map(command => command.CreateBy, upsertCommand => upsertCommand.UpsertBy)
            .Map(command => command.CreateAt, upsertCommand => upsertCommand.UpsertAt)
            .Map(command => command.IsPreChecked, upsertCommand => true);
            
        config.NewConfig<UpsertProductCommand, UpdateProductCommand>()
            .Map(command => command.UpdateBy, upsertCommand => upsertCommand.UpsertBy)
            .Map(command => command.UpdateAt, upsertCommand => upsertCommand.UpsertAt)
            .Map(command => command.IsPreChecked, upsertCommand => true);

        config.NewConfig<ProductAggregate, ProductResponse>()
            .Map(response => response.Owner, product => product.Customers
                .Select(pc => pc.Customer)
                .FirstOrDefault(c => c != null && c.Id == product.OwnerId))
            .Map(response => response.Dealer, product => product.Customers
                .Select(pc => pc.Customer)
                .FirstOrDefault(c => c != null && c.Id == product.DealerId));
        
        config.NewConfig<ProductCreatedEvent, ProductCreated>()
            .Map(response => response.Owner, @event => @event.OwnerId == null ? null : @event.OwnerId)
            .Map(response => response.Dealer, @event => @event.DealerId == null ? null : @event.DealerId);
        
        config.NewConfig<ProductOwnerChangedEvent, ProductOwnerChanged>()
            .Map(response => response.Owner, @event => @event.OwnerId == null ? null : @event.OwnerId);
        
        config.NewConfig<ProductDealerChangedEvent, ProductDealerChanged>()
            .Map(response => response.Dealer, @event => @event.DealerId == null ? null : @event.DealerId);
    }
}