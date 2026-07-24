using FooBar.Domain.Customers.Entity;
using FooBar.Domain.Invoices.Model.Entity;

namespace FooBar.Api.Tests.InvoiceApi
{
    public class InvoiceDataBuilder
    {
        Guid _id;
        Customer _customer = new CustomerDataBuilder().Build();
        ICollection<ProductInvoice> _products = [new ProductInvoiceDataBuilder().Build()];
        InvoiceState _state = InvoiceState.Active;

        public InvoiceDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public InvoiceDataBuilder WithCustomer(Customer customer)
        {
            _customer = customer;
            return this;
        }

        public InvoiceDataBuilder WithProductsInvoice(ICollection<ProductInvoice> productInvoices)
        {
            _products = productInvoices;
            return this;
        }

        public InvoiceDataBuilder WithState(InvoiceState state)
        {
            _state = state;
            return this;
        }

        public Invoice Build()
        {
            return new Invoice()
            {
                Id = _id,
                Customer = _customer!,
                ProductsInvoice = _products,
                State = _state
            };
        }
    }
}
