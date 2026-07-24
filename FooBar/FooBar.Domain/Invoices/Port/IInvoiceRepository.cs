using FooBar.Domain.Invoices.Model.Entity;

namespace FooBar.Domain.Invoices.Port
{
    public interface IInvoiceRepository
    {
        Task<Guid> AddAsync(Invoice invoice);
        Task<Invoice> GetByIdAsync(Guid id, string? include = default);
        void Update(Invoice invoice);
    }
}
