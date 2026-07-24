using FooBar.Domain.Common;
using FooBar.Domain.Invoices.Port;

namespace FooBar.Domain.Invoices.Service
{
    [DomainService]
    public class CancelInvoiceService(IInvoiceRepository invoiceRepository)
    {
        public async Task ExecuteAsync(Guid id)
        {
            var invoice = await invoiceRepository.GetByIdAsync(id, nameof(Customers.Entity.Customer));
            invoice.ValidateNull("the invoice not exist.");
            invoice.IsCancel();
            invoice.Cancel();
            invoiceRepository.Update(invoice);
        }
    }
}
