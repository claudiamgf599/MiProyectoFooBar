namespace FooBar.Application.Invoice.Command
{
    public record ProductInvoiceCommand(Guid ProductId, int Quantity);

}
