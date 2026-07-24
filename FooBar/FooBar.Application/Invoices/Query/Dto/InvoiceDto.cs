namespace FooBar.Application.Invoice.Query.Dto
{
    public record InvoiceDto
    {
        public Guid Id { get; set; }
        public decimal ValueTotal { get; set; }
        public string State { get; set; } = default!;
        public CustomerDto Customer { get; set; } = default!;
        public IEnumerable<ProductInvoiceDto> ProductsInvoice { get; set; } = default!;

    }
}
