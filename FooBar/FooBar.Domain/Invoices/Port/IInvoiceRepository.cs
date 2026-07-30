using FooBar.Domain.Invoices.Model.Entity;
using System.Linq.Expressions;

namespace FooBar.Domain.Invoices.Port
{
    public interface IInvoiceRepository
    {
        Task<Guid> AddAsync(Invoice invoice);
        Task<Invoice> GetByIdAsync(Guid id, string? include = default);
        void Update(Invoice invoice);
        Task<IEnumerable<Invoice>> GetManyAsync(Expression<Func<Invoice, bool>>? filter = null);
    }
}
