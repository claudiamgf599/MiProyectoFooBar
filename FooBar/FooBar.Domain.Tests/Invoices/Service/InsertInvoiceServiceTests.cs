using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;
using FooBar.Domain.Invoices.Service;
using FooBar.Domain.Tests.Customers.Entity;
using FooBar.Domain.Tests.Invoices.Model.Entity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FooBar.Domain.Tests.Invoices.Service
{
    public class InsertInvoiceServiceTests
    {
        readonly IInvoiceRepository _invoiceRepository;
        readonly InsertInvoiceService _insertInvoiceService;

        public InsertInvoiceServiceTests()
        {
            _invoiceRepository = Substitute.For<IInvoiceRepository>();
            _insertInvoiceService = new InsertInvoiceService(_invoiceRepository);
        }

        [Fact]
        public async Task ExecuteAsync_Success()
        {
            var customer = new CustomerDataBuilder().Build();
            ICollection<ProductInvoice> productsInvoice = new List<ProductInvoice>
            {
                new ProductInvoiceDataBuilder().Build(),
                new ProductInvoiceDataBuilder().Build()
            };
            var invoice = new InvoiceDataBuilder()
                .WithCustomer(customer)
                .WithProductsInvoice(productsInvoice)
                .Build();
            var id = Guid.NewGuid();
            _invoiceRepository.AddAsync(Arg.Any<Invoice>()).Returns(id);

            var InvoiceId = await _insertInvoiceService.ExecuteAsync(invoice);

            await _invoiceRepository.Received(1).AddAsync(Arg.Any<Invoice>());
            Assert.Equal(id, InvoiceId);
        }
    }
}
