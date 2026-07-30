using MediatR;

namespace FooBar.Application.Invoice.Command
{
    /// <summary>
    /// Command para agregar o actualizar una nota en una factura.
    /// Usa 'record' para inmutabilidad y value semantics.
    /// </summary>
    public record UpdateInvoiceNoteCommand(
        Guid Id, 
        string Note
    ) : IRequest<Unit>;
}