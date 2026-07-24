using FooBar.Domain.Common;
using FooBar.Domain.Customers.Port;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Products.Port;

namespace FooBar.Application.Invoice.Command.Factory
{
    public class InvoiceFactory(ICustomerRepository customerRepository, IProductRepository productRepository)
    {
        public async Task<Domain.Invoices.Model.Entity.Invoice> CreateAsync(InsertInvoiceCommand insertInvoiceCommand)
        {
            var customer = await customerRepository.GetByIdAsync(insertInvoiceCommand.CustomerId);
            customer.ValidateNull($"the customer with id {insertInvoiceCommand.CustomerId} does not exist.");

            ICollection<ProductInvoice> productInvoices = [];
            foreach (var productInvoice in insertInvoiceCommand.ProductsInvoice)
            {
                var product = await productRepository.GetByIdAsync(productInvoice.ProductId);
                product.ValidateNull($"the product with id {productInvoice.ProductId} does not exist.");
                productInvoices.Add(new ProductInvoice() { Product = product, Quantity = productInvoice.Quantity });
            }

            Domain.Invoices.Model.Entity.Invoice invoice = new()
            {
                Customer = customer,
                ProductsInvoice = productInvoices,
                State = InvoiceState.Active
            };

            return invoice;
        }
    }
}
