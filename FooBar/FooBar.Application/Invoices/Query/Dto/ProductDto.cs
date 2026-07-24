namespace FooBar.Application.Invoice.Query.Dto
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public bool ApplyIva { get; set; } = default!;
        public decimal Value { get; set; }
    }
}
