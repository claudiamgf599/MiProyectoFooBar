using FooBar.Domain.Exceptions;
using FooBar.Domain.Tests.Customers.Entity;

namespace FooBar.Domain.Tests.Invoices.Model.Entity
{
    public class InvoiceTests
    {
        [Fact]
        public void Invoice_WithCustomerNull_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new InvoiceDataBuilder().WithCustomer(default!).Build());

            Assert.Equal("the customer should not be null.", exception.Message);
        }

        [Fact]
        public void Invoice_WithProductsInvoiceNull_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new InvoiceDataBuilder().WithProductsInvoice(default!).Build());

            Assert.Equal("the products should not be null.", exception.Message);
        }

        [Fact]
        public void Invoice_WithProductsInvoiceEmpty_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new InvoiceDataBuilder().WithProductsInvoice(new List<Domain.Invoices.Model.Entity.ProductInvoice>()).Build());

            Assert.Equal("the products should not be empty.", exception.Message);
        }

        [Fact]
        public void Invoice_WithStateInvalid_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new InvoiceDataBuilder().WithState((Domain.Invoices.Model.Entity.InvoiceState)9).Build());

            Assert.Equal("the invoice state is not valid.", exception.Message);
        }

        [Fact]
        public void Invoice_CustomerIsSpecial_Success()
        {
            ICollection<Domain.Invoices.Model.Entity.ProductInvoice> productsInvoice = new List<Domain.Invoices.Model.Entity.ProductInvoice>
            {
                new ProductInvoiceDataBuilder().Build(),
                new ProductInvoiceDataBuilder().Build()
            };

            var invoice = new InvoiceDataBuilder()
                .WithProductsInvoice(productsInvoice)
                .Build();

            Assert.Equal(642.6M, invoice.ValueTotal);
        }

        [Fact]
        public void Invoice_CustomerIsPreferential_Success()
        {
            var customer = new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Preferential).Build();
            ICollection<Domain.Invoices.Model.Entity.ProductInvoice> productsInvoice = new List<Domain.Invoices.Model.Entity.ProductInvoice>
            {
                new ProductInvoiceDataBuilder().Build(),
                new ProductInvoiceDataBuilder().Build()
            };

            var invoice = new InvoiceDataBuilder()
                .WithCustomer(customer)
                .WithProductsInvoice(productsInvoice)
                .Build();

            Assert.Equal(571.2M, invoice.ValueTotal);
        }

        [Fact]
        public void Invoice_CustomerIsCommun_Success()
        {
            var customer = new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Common).Build();
            ICollection<Domain.Invoices.Model.Entity.ProductInvoice> productsInvoice = new List<Domain.Invoices.Model.Entity.ProductInvoice>
            {
                new ProductInvoiceDataBuilder().Build(),
                new ProductInvoiceDataBuilder().Build()
            };

            var invoice = new InvoiceDataBuilder()
                .WithCustomer(customer)
                .WithProductsInvoice(productsInvoice)
                .Build();

            Assert.Equal(714M, invoice.ValueTotal);
        }
    }
}
