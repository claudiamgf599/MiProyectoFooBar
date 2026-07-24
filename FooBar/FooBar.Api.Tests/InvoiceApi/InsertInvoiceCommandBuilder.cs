using FooBar.Application.Invoice.Command;

namespace FooBar.Api.Tests.InvoiceApi
{
    public class InsertInvoiceCommandBuilder
    {
        Guid _customerId;
        IEnumerable<ProductInvoiceCommand> _productsInvoice;

        public InsertInvoiceCommand Build() => new(_customerId, _productsInvoice);

        public InsertInvoiceCommandBuilder WithCustomerId(Guid customerId)
        {
            _customerId = customerId;
            return this;
        }

        public InsertInvoiceCommandBuilder WithProductsInvoice(IEnumerable<ProductInvoiceCommand> productInvoices)
        {
            _productsInvoice = productInvoices;
            return this;
        }
    }
}
