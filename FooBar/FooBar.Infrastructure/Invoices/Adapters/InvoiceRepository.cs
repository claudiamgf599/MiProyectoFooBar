using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;
using FooBar.Infrastructure.Adapters;
using FooBar.Infrastructure.Port;

namespace FooBar.Infrastructure.Invoices.Adapters
{
    [Repository]
    public class InvoiceRepository(IRepository<Invoice> invoiceRepository) : IInvoiceRepository
    {
        public async Task<Invoice> GetByIdAsync(Guid id) => await invoiceRepository.GetOneAsync(id);

        public async Task<Guid> AddAsync(Invoice invoice)
        {
            var invoiceInsert = await invoiceRepository.AddAsync(invoice);
            return invoiceInsert.Id;
        }

        public void Update(Invoice invoice) => invoiceRepository.UpdateAsync(invoice);

        public Task<Invoice> GetByIdAsync(Guid id, string? include = default)
        {
            return invoiceRepository.GetOneAsync(id, include);
        }
    }
}
