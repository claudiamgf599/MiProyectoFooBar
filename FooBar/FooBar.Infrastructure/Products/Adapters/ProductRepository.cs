using FooBar.Domain.Products.Entity;
using FooBar.Domain.Products.Port;
using FooBar.Infrastructure.Adapters;
using FooBar.Infrastructure.Port;

namespace FooBar.Infrastructure.Products.Adapters
{
    [Repository]
    public class ProductRepository(IRepository<Product> productRepository) : IProductRepository
    {
        public async Task<Product> AddAsync(Product product) => await productRepository.AddAsync(product);

        public async Task DeleteAsync(Product product) => await Task.Run(() => productRepository.DeleteAsync(product));

        public async Task<IEnumerable<Product>> GetAllAsync() => await productRepository.GetManyAsync();


        public async Task<Product> GetByIdAsync(Guid id) => await productRepository.GetOneAsync(id);

        public async Task<int> GetCountAsync() => await productRepository.GetCountAsync();

        public async Task UpdateAsync(Product product) => await Task.Run(() => productRepository.UpdateAsync(product));
    }
}
