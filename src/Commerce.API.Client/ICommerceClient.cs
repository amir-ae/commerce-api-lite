using Commerce.API.Client.Resources.Interfaces;

namespace Commerce.API.Client
{
    public interface ICommerceClient
    {
        IProductResource Product { get; }
        ICustomerResource Customer { get; }
    }
}