using FooBar.Domain.Products.Entity;

namespace FooBar.Domain.Products.Port
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product> GetByIdAsync(Guid id);
        Task<int> GetCountAsync();
        Task<IEnumerable<Product>> GetAllAsync();
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
