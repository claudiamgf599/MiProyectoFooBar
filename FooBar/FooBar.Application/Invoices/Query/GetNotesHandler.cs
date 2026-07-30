using FooBar.Domain.Invoices.Model.Dto;
using FooBar.Domain.Invoices.Port;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    /// <summary>
    /// Handler que obtiene todas las facturas que tienen una nota.
    /// 
    /// Usa Dapper a través de IInvoiceSimpleQueryRepository para una consulta rápida.
    /// No necesita AutoMapper porque el query ya retorna NoteDto del Domain.
    /// </summary>
    public class GetNotesHandler(
        IInvoiceSimpleQueryRepository invoiceSimpleQueryRepository
    ) : IRequestHandler<GetNotesQuery, IEnumerable<NoteDto>>
    {
        public async Task<IEnumerable<NoteDto>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
        {
            // Usar Dapper query que retorna NoteDto directamente (sin AutoMapper)
            return await invoiceSimpleQueryRepository.GetAllWithNotesAsync();
        }
    }
}