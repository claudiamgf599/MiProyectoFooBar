using FooBar.Domain.Invoices.Model.Dto;

namespace FooBar.Domain.Invoices.Port
{
    public interface IInvoiceSimpleQueryRepository
    {
        Task<IEnumerable<SummaryInvoiceDto>> GetAllCancelAsync();
    }
}
