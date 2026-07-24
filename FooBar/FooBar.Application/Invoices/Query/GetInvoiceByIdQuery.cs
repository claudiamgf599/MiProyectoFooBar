using FooBar.Application.Invoice.Query.Dto;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    public record GetInvoiceByIdQuery(Guid id) : IRequest<InvoiceDto>;
}
