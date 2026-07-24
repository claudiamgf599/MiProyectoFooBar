using FooBar.Application.Ports;
using FooBar.Domain.Customers.Port;
using FooBar.Domain.Products.Port;
using FooBar.Infrastructure.DataSource.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FooBar.Infrastructure.Extensions
{
    public static class SeedExtension
    {
        public static IHost Seed(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            var productRepository = services.GetRequiredService<IProductRepository>();
            var initializerProduct = new InitializerProduct(productRepository, unitOfWork);
            initializerProduct.CreateAsync().Wait();

            var customerRepository = services.GetRequiredService<ICustomerRepository>();
            var initializerCustomer = new InitializerCustomer(customerRepository, unitOfWork);
            initializerCustomer.CreateAsync().Wait();

            return host;
        }
    }
}
