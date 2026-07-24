using MediatR;

namespace FooBar.Application.Invoice.Command
{
    public record CancelInvoiceCommand(Guid id) : IRequest<Unit>;

}
