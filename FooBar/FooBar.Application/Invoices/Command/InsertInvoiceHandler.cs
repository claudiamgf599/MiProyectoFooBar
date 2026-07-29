using FooBar.Application.Invoice.Command.Factory;
using FooBar.Application.Ports;
using FooBar.Domain.Invoices.Service;
using MediatR;

namespace FooBar.Application.Invoice.Command
{
    internal class InsertInvoiceHandler(InvoiceFactory insertInvoiceFactory, InsertInvoiceService insertInvoiceService, IUnitOfWork unitOfWork) : IRequestHandler<InsertInvoiceCommand, Guid>
    {
        // TODO - para validar con la IA
        public async Task<Guid> Handle(InsertInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await insertInvoiceFactory.CreateAsync(request);
            var invoiceId = await insertInvoiceService.ExecuteAsync(invoice);
            await unitOfWork.SaveAsync();
            return invoiceId;
        }
    }
}
