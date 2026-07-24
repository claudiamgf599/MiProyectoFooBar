using MediatR;

namespace FooBar.Application.Invoice.Command
{
    public record InsertInvoiceCommand(
        Guid CustomerId,
        IEnumerable<ProductInvoiceCommand> ProductsInvoice
    ) : IRequest<Guid>;
}
