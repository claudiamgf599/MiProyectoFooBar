namespace FooBar.Domain.Invoices.Model.Dto
{
    public record InvoiceSingleDto
    {
        public Guid Id { get; set; }
        public decimal ValueTotal { get; set; }
        public string State { get; set; } = default!;
        public Guid CustomerId { get; set; }
    }
}
