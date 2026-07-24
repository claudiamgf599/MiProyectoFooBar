using AutoMapper;
using FooBar.Application.Invoice.Query.Dto;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    internal class GetInvoiceByIdHandler(IInvoiceRepository invoiceRepository, IMapper mapper) : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            string includeProductsInvoice = nameof(Domain.Invoices.Model.Entity.Invoice.ProductsInvoice);
            string includeCustomes = nameof(Domain.Invoices.Model.Entity.Invoice.Customer);
            string includeProduct = $"{nameof(Domain.Invoices.Model.Entity.Invoice.ProductsInvoice)}.{nameof(ProductInvoice.Product)}";
            var invoice = await invoiceRepository.GetByIdAsync(request.id, $"{includeProductsInvoice},{includeCustomes},{includeProduct}");

            return mapper.Map<InvoiceDto>(invoice);
        }
    }
}
