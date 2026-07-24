namespace FooBar.Application.Invoice.Query.Dto
{
    public class ProductInvoiceDto
    {
        public ProductDto Product { get; set; } = default!;
        public decimal Quantity { get; set; } = default!;
    }
}
