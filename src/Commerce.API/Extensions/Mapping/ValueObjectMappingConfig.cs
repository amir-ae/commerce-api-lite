using Commerce.API.Contract.V1.Common.Requests;
using Commerce.API.Contract.V1.Customers.Responses.Models;
using Mapster;
using Commerce.Domain.Common.ValueObjects;
using Commerce.Domain.Customers.ValueObjects;
using Commerce.Domain.Products.ValueObjects;
using Customer = Commerce.API.Contract.V1.Products.Responses.Models.Customer;
using CustomerRole = Commerce.Domain.Customers.ValueObjects.CustomerRole;
using CustomerRoleEnum = Commerce.API.Contract.V1.Common.Models.CustomerRole;
using OrderIdResponse = Commerce.API.Contract.V1.Common.Responses.OrderId;
using PhoneNumberEnum = Commerce.API.Contract.V1.Common.Requests.PhoneNumber;
using PhoneNumber = Commerce.Domain.Customers.ValueObjects.PhoneNumber;
using PhoneNumberResponse = Commerce.API.Contract.V1.Customers.Responses.Models.PhoneNumber;

namespace Commerce.API.Extensions.Mapping;

public class ValueObjectMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region
        
        config.NewConfig<Guid, AppUserId>()
            .MapWith(id => new AppUserId(id));
        
        config.NewConfig<int, CityId>()
            .MapWith(id => new CityId(id));
        
        config.NewConfig<int?, CityId?>()
            .MapWith(id => id.HasValue ? new CityId(id.Value) : null);
        
        config.NewConfig<string, CountryId>()
            .MapWith(id => new CountryId(id));
        
        config.NewConfig<string?, CountryId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new CountryId(id) : null);
        
        config.NewConfig<string, CustomerId>()
            .MapWith(id => new CustomerId(id));
        
        config.NewConfig<string?, CustomerId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new CustomerId(id) : null);

        config.NewConfig<string, ProductId>()
            .MapWith(id => new ProductId(id));
        
        config.NewConfig<string?, ProductId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new ProductId(id) : null);
        
        config.NewConfig<int, SerialId>()
            .MapWith(id => new SerialId(id));
        
        config.NewConfig<int?, SerialId?>()
            .MapWith(id => id.HasValue ? new SerialId(id.Value) : null);

        config.NewConfig<string, OrderId>()
            .MapWith(id => new OrderId(id));
        
        config.NewConfig<string?, OrderId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new OrderId(id) : null);
        
        config.NewConfig<string, CentreId>()
            .MapWith(id => new CentreId(id));
        
        config.NewConfig<string?, CentreId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new CentreId(id) : null);

        config.NewConfig<CustomerRoleEnum, CustomerRole>()
            .MapWith(role => CustomerRole.FromValue((int)role));
        
        config.NewConfig<CustomerRoleEnum?, CustomerRole?>()
            .MapWith(role => role.HasValue ? CustomerRole.FromValue((int)role.Value) : null);
        
        config.NewConfig<PhoneNumberEnum, PhoneNumber>()
            .MapWith(phoneNumber => new PhoneNumber(phoneNumber.Value, 
                !string.IsNullOrWhiteSpace(phoneNumber.CountryId) ? new CountryId(phoneNumber.CountryId) : null,
                phoneNumber.CountryCode, phoneNumber.Description));
        
        config.NewConfig<PhoneNumberEnum?, PhoneNumber?>()
            .MapWith(phoneNumber => phoneNumber == null ? null : new PhoneNumber(phoneNumber.Value, 
                !string.IsNullOrWhiteSpace(phoneNumber.CountryId) ? new CountryId(phoneNumber.CountryId) : null,
                phoneNumber.CountryCode, phoneNumber.Description));

        config.NewConfig<Order, OrderId>()
            .MapWith(id => new OrderId(id.OrderId, id.CentreId));
        
        config.NewConfig<ProductOrderLink, ProductOrderLink>()
            .Ignore(po => po.Product!);

        config.NewConfig<CustomerOrderLink, CustomerOrderLink>()
            .Ignore(co => co.Customer!);

        config.NewConfig<CustomerProductLink, CustomerProductLink>()
            .Ignore(cp => cp.Customer!)
            .Ignore(cp => cp.Product!);


        #endregion
        #region
        
        config.NewConfig<AppUserId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<AppUserId?, Guid?>()
            .MapWith( id => id == null ? null : id.Value);

        config.NewConfig<CityId, int>()
            .MapWith(id => id.Value);

        config.NewConfig<CityId, City>()
            .MapWith(id => new City(id.Value));
        
        config.NewConfig<CountryId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CustomerId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CustomerId?, string?>()
            .MapWith(id => id == null ? null : id.Value);
        
        config.NewConfig<CustomerId, Customer>()
            .MapWith(id => new Customer(id.Value));
        
        config.NewConfig<CustomerId?, Customer?>()
            .MapWith(id => id == null ? null : new Customer(id.Value));
        
        config.NewConfig<CustomerOrderLink, Customer>()
            .MapWith(co => co.Customer == null ? new Customer(co.CustomerId.Value) : 
                new Customer(co.Customer.Id.Value, co.Customer.FullName, (CustomerRoleEnum)co.Customer.Role.Value, 
                    co.Customer.FirstName, co.Customer.MiddleName, co.Customer.LastName, 
                    new PhoneNumberResponse(co.Customer.PhoneNumber.Value, co.Customer.PhoneNumber.CountryId.Value, 
                        co.Customer.PhoneNumber.CountryCode, co.Customer.PhoneNumber.Description), 
                    new City(co.Customer.CityId.Value), co.Customer.Address));

        config.NewConfig<ProductId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<OrderId, OrderIdResponse>()
            .MapWith(id => new OrderIdResponse(id.Value, id.CentreId));
        
        config.NewConfig<CentreId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CentreId?, string?>()
            .MapWith( id => id == null ? null : id.Value);
        
        config.NewConfig<SerialId, int>()
            .MapWith(id => id.Value);
        
        config.NewConfig<SerialId?, int?>()
            .MapWith(id => id == null ? null : id.Value);

        config.NewConfig<CustomerRole, CustomerRoleEnum>()
            .MapWith(role => (CustomerRoleEnum)role.Value);
        
        config.NewConfig<CustomerRole?, CustomerRoleEnum?>()
            .MapWith(role => role == null ? null : (CustomerRoleEnum)role.Value);
        
        #endregion
    }
}