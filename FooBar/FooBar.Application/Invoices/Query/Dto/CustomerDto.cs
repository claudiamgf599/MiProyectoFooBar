namespace FooBar.Application.Invoice.Query.Dto
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string TypeCustomer { get; set; } = default!;
    }
}
