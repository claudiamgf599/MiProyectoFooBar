using FooBar.Application.Ports;
using FooBar.Domain.Common;
using FooBar.Domain.Invoices.Port;
using MediatR;

namespace FooBar.Application.Invoice.Command
{
    /// <summary>
    /// Handler que orquesta la actualización de una nota en una factura.
    /// 
    /// Patrón de inyección con Primary Constructor (C# 12):
    /// - Las dependencias se declaran en el constructor y el compilador genera los campos readonly
    /// </summary>
    public class UpdateInvoiceNoteHandler(
        IInvoiceRepository invoiceRepository, 
        IUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateInvoiceNoteCommand, Unit>
    {
        /// <summary>
        /// Maneja el command UpdateInvoiceNoteCommand.
        /// 
        /// Flujo:
        /// 1. Buscar la factura por ID
        /// 2. Validar que exista
        /// 3. Llamar al método de dominio SetNote() (encapsula reglas de negocio)
        /// 4. Persistir cambios con UnitOfWork
        /// </summary>
        public async Task<Unit> Handle(UpdateInvoiceNoteCommand request, CancellationToken cancellationToken)
        {
            // Buscar la factura por ID
            var invoice = await invoiceRepository.GetByIdAsync(request.Id);
            
            // Validar que la factura exista
            invoice.ValidateNull("the invoice does not exist.");
            
            // Llamar al método de dominio que encapsula las reglas de negocio:
            // - Nota no puede ser null o vacía
            // - Nota no puede exceder 500 caracteres
            // - Factura no puede estar cancelada
            invoice.SetNote(request.Note);
            
            // Persistir los cambios en la base de datos
            await unitOfWork.SaveAsync(cancellationToken);
            
            // Unit es un tipo que representa "sin valor" (equivalente a void pero como tipo)
            return Unit.Value;
        }
    }
}