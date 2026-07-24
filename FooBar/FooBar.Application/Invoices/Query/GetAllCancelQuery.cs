using FooBar.Domain.Invoices.Model.Dto;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    public record GetAllCancelQuery() : IRequest<IEnumerable<SummaryInvoiceDto>>;

}
