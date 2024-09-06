using Commerce.API.Contract.V1.Customers.Requests;
using Mapster;
using Commerce.API.Contract.V1.Customers.Responses;
using Commerce.API.Contract.V1.Customers.Responses.Events;
using Commerce.Application.Customers.Commands.Create;
using Commerce.Application.Customers.Commands.Update;
using Commerce.Application.Customers.Commands.Upsert;
using Commerce.Domain.Customers.Events;
using CustomerAggregate = Commerce.Domain.Customers.Customer;
using ProductAggregate = Commerce.Domain.Products.Product;

namespace Commerce.API.Extensions.Mapping;

public class CustomerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateCustomerRequest, CreateCustomerCommand>()
            .Map(command => command.ProductIds,
                request => request.Products == null ? null : request.Products.Select(p => p.ProductId).ToHashSet())
            .Map(command => command.OrderIds, request => request.Orders);

        config.NewConfig<UpdateCustomerRequest, UpdateCustomerCommand>()
            .Map(command => command.ProductIds,
                request => request.Products == null ? null : request.Products.Select(p => p.ProductId).ToHashSet())
            .Map(command => command.OrderIds, request => request.Orders);

        config.NewConfig<UpsertCustomerCommand, CreateCustomerCommand>()
            .Map(command => command.CreateBy, upsertCommand => upsertCommand.UpsertBy)
            .Map(command => command.CreateAt, upsertCommand => upsertCommand.UpsertAt)
            .Map(command => command.IsPreChecked, upsertCommand => true);

        config.NewConfig<UpsertCustomerCommand, UpdateCustomerCommand>()
            .Map(command => command.UpdateBy, upsertCommand => upsertCommand.UpsertBy)
            .Map(command => command.UpdateAt, upsertCommand => upsertCommand.UpsertAt)
            .Map(command => command.IsPreChecked, upsertCommand => true);

        config.NewConfig<CustomerAggregate, CustomerBriefResponse>()
            .Map(response => response.City, customer => customer.CityId);

        config.NewConfig<CustomerAggregate, CustomerResponse>()
            .Map(response => response.City, customer => customer.CityId)
            .Map(response => response.Products, customer => customer.Products.Select(cp => cp.Product));
        
        config.NewConfig<CustomerCreatedEvent, CustomerCreated>()
            .Map(response => response.City, @event => @event.CityId);
        
        config.NewConfig<CustomerAddressChangedEvent, CustomerAddressChanged>()
            .Map(response => response.City, @event => @event.CityId);
    }
}