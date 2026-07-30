using FooBar.Application.Invoice.Query.Dto;
using FooBar.Domain.Invoices.Model.Dto;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;
using FooBar.Infrastructure.Adapters;
using FooBar.Infrastructure.DataSource;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FooBar.Infrastructure.Invoices.Adapters
{
    [Repository]
    public class InvoiceSimpleQueryRepository(DataContext dataContext) : IInvoiceSimpleQueryRepository
    {
        private IDbConnection DbConnection => dataContext.Database.GetDbConnection();

        public async Task<IEnumerable<SummaryInvoiceDto>> GetAllCancelAsync()
        {
            var invoices = await DbConnection
                .QueryAsync<SummaryInvoiceDto>(@"select id, ValueTotal, State from Invoice where State = @State",
                    new { State = InvoiceState.Canceled }
                    );
            return invoices;
        }

        public async Task<IEnumerable<NoteDto>> GetAllWithNotesAsync()
        {
            // Dapper ejecuta SQL directo y mapea columnas → propiedades por nombre
            // Nota: la columna 'Note' puede ser NULL en la base de datos
            var notes = await DbConnection
                .QueryAsync<NoteDto>(@"select id, ValueTotal, State, Note from Invoice where Note is not null and Note != ''");
            return notes;
        }
    }
}
