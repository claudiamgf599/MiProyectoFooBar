using FooBar.Application.Ports;
using FooBar.Domain.Invoices.Service;
using MediatR;

namespace FooBar.Application.Invoice.Command
{
    internal class CancelInvoiceHandler(CancelInvoiceService cancelInvoiceService, IUnitOfWork unitOfWork) : IRequestHandler<CancelInvoiceCommand, Unit>
    {
        public async Task<Unit> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            await cancelInvoiceService.ExecuteAsync(request.id);
            await unitOfWork.SaveAsync();
            return Unit.Value;
        }
    }
}
