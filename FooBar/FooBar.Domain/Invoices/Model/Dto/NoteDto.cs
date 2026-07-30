namespace FooBar.Domain.Invoices.Model.Dto
{
    /// <summary>
    /// DTO ligero para retornar facturas con notas.
    /// Se usa en queries simples con Dapper.
    /// </summary>
    public record NoteDto
    {
        public Guid Id { get; set; }
        public string? Note { get; set; }
        public decimal ValueTotal { get; set; }
        public string State { get; set; } = default!;
    }
}