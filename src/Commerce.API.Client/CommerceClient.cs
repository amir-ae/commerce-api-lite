using Commerce.API.Client.Base;
using Commerce.API.Client.Resources;
using Commerce.API.Client.Resources.Interfaces;

namespace Commerce.API.Client
{
    public class CommerceClient : ICommerceClient
    {
        public CommerceClient(HttpClient client)
        {
            Product = new ProductResource(new BaseClient(client, client.BaseAddress!.ToString()));
            Customer = new CustomerResource(new BaseClient(client, client.BaseAddress!.ToString()));
        }
        public IProductResource Product { get; private set; }
        public ICustomerResource Customer { get; private set; }
    }
}