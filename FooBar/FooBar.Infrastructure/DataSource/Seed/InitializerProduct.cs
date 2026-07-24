using FooBar.Application.Ports;
using FooBar.Domain.Products.Entity;
using FooBar.Domain.Products.Port;

namespace FooBar.Infrastructure.DataSource.Seed
{
    public class InitializerProduct(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        public async Task CreateAsync()
        {
            var count = await productRepository.GetCountAsync();
            if (count > 0) return;

            Product product = new() { Id = Guid.NewGuid(), Name = "Service", ApplyIva = true, Value = 50000 };
            await productRepository.AddAsync(product);
            await unitOfWork.SaveAsync();
        }

    }
}
