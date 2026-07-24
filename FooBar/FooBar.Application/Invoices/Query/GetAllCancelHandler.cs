using FooBar.Domain.Invoices.Model.Dto;
using FooBar.Domain.Invoices.Port;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    internal class GetAllCancelHandler(IInvoiceSimpleQueryRepository invoiceSimpleQueryRepository) : IRequestHandler<GetAllCancelQuery, IEnumerable<SummaryInvoiceDto>>
    {
        public async Task<IEnumerable<SummaryInvoiceDto>> Handle(GetAllCancelQuery request, CancellationToken cancellationToken)
        {
            return await invoiceSimpleQueryRepository.GetAllCancelAsync();
        }
    }
}
