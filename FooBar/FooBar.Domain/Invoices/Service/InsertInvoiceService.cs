using FooBar.Domain.Common;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;

namespace FooBar.Domain.Invoices.Service
{
    [DomainService]
    public class InsertInvoiceService(IInvoiceRepository invoiceRepository)
    {
        public async Task<Guid> ExecuteAsync(Invoice invoice)
        {
            var invoiceId = await invoiceRepository.AddAsync(invoice);
            
            return invoiceId;
        }
    }
}
