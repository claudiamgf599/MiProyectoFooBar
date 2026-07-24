using FooBar.Domain.Customers.Entity;
using FooBar.Domain.Customers.Port;
using FooBar.Infrastructure.Adapters;
using FooBar.Infrastructure.Port;

namespace FooBar.Infrastructure.Customers.Adapters
{
    [Repository]
    public class CustomerRepository(IRepository<Customer> custumerRepository) : ICustomerRepository
    {
        public async Task<Customer> AddAsync(Customer customer) => await custumerRepository.AddAsync(customer);

        public async Task<Customer> GetByIdAsync(Guid id) => await custumerRepository.GetOneAsync(id);

        public async Task<int> GetCountAsync() => await custumerRepository.GetCountAsync();
    }
}
