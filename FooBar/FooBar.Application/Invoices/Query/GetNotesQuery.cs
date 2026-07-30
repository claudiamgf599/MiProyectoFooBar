using FooBar.Domain.Invoices.Model.Dto;
using MediatR;

namespace FooBar.Application.Invoice.Query
{
    /// <summary>
    /// Query para obtener todas las facturas que tienen una nota.
    /// </summary>
    public record GetNotesQuery() : IRequest<IEnumerable<NoteDto>>;
}